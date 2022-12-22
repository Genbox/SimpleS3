using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Errors;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

/// <summary>Handles common request and response logic before sending to transport drivers.</summary>
internal class DefaultRequestHandler : IRequestHandler
{
    private readonly IAuthorizationBuilder _authBuilder;
    private readonly SimpleS3Config _config;
    private readonly IEndpointBuilder _endpointBuilder;
    private readonly ILogger<DefaultRequestHandler> _logger;
    private readonly IMarshalFactory _marshaller;
    private readonly INetworkDriver _networkDriver;
    private readonly IPostMapperFactory _postMapper;
    private readonly IList<IRequestStreamWrapper> _requestStreamWrappers;
    private readonly IRequestValidatorFactory _requestValidator;

    public DefaultRequestHandler(IOptions<SimpleS3Config> options, IRequestValidatorFactory validator, IMarshalFactory marshaller, IPostMapperFactory postMapper, INetworkDriver networkDriver, HeaderAuthorizationBuilder authBuilder, IEndpointBuilder endpointBuilder, ILogger<DefaultRequestHandler> logger, IEnumerable<IRequestStreamWrapper>? requestStreamWrappers = null)
    {
        Validator.RequireNotNull(options);
        Validator.RequireNotNull(validator);
        Validator.RequireNotNull(marshaller);
        Validator.RequireNotNull(networkDriver);
        Validator.RequireNotNull(authBuilder);
        Validator.RequireNotNull(logger);

        _requestValidator = validator;
        _config = options.Value;
        _networkDriver = networkDriver;
        _authBuilder = authBuilder;
        _endpointBuilder = endpointBuilder;
        _marshaller = marshaller;
        _postMapper = postMapper;
        _logger = logger;

        if (requestStreamWrappers == null)
            _requestStreamWrappers = Array.Empty<IRequestStreamWrapper>();
        else
            _requestStreamWrappers = requestStreamWrappers.ToList();
    }

    public Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken token = default) where TReq : IRequest where TResp : IResponse, new()
    {
        token.ThrowIfCancellationRequested();

        if (request is SignedBaseRequest preSigned)
            return SendPreSignedAsync<TResp>(preSigned, token);

        return SendRequestInternalAsync<TReq, TResp>(request, token);
    }

    private async Task<TResp> SendPreSignedAsync<TResp>(SignedBaseRequest preSigned, CancellationToken token) where TResp : IResponse, new()
    {
        using Stream? requestStream = _marshaller.MarshalRequest(_config, preSigned);
        return await HandleResponseAsync<SignedBaseRequest, TResp>(preSigned, preSigned.Url, requestStream, token).ConfigureAwait(false);
    }

    private Task<TResp> SendRequestInternalAsync<TReq, TResp>(TReq request, CancellationToken token) where TReq : IRequest where TResp : IResponse, new()
    {
        request.Timestamp = DateTimeOffset.UtcNow;
        request.RequestId = Guid.NewGuid();

        _logger.LogTrace("Handling {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

        Stream? requestStream = _marshaller.MarshalRequest(_config, request);

        _requestValidator.ValidateAndThrow(request);

        IEndpointData endpointData = _endpointBuilder.GetEndpoint(request);
        request.SetHeader(HttpHeaders.Host, endpointData.Host);
        request.SetHeader(AmzHeaders.XAmzDate, request.Timestamp, DateTimeFormat.Iso8601DateTime);

        if (requestStream != null)
        {
            foreach (IRequestStreamWrapper wrapper in _requestStreamWrappers)
            {
                if (wrapper.IsSupported(request))
                    requestStream = wrapper.Wrap(requestStream, request);
            }
        }

        if (!request.Headers.TryGetValue(AmzHeaders.XAmzContentSha256, out string? contentHash))
        {
            if (_config.PayloadSignatureMode == SignatureMode.Unsigned)
                contentHash = "UNSIGNED-PAYLOAD";
            else
                contentHash = requestStream == null ? Constants.EmptySha256 : CryptoHelper.Sha256Hash(requestStream, true).HexEncode();

            request.SetHeader(AmzHeaders.XAmzContentSha256, contentHash);
        }

        _logger.LogDebug("ContentSha256 is {ContentSha256}", contentHash);

        //We add the authorization header here because we need ALL other headers to be present when we do
        _authBuilder.BuildAuthorization(request);

        StringBuilder sb = StringBuilderPool.Shared.Rent(200);
        sb.Append(endpointData.Endpoint);
        RequestHelper.AppendPath(sb, _config, request);
        RequestHelper.AppendQueryParameters(sb, request);
        string url = StringBuilderPool.Shared.ReturnString(sb);

        return HandleResponseAsync<TReq, TResp>(request, url, requestStream, token);
    }

    private async Task<TResp> HandleResponseAsync<TReq, TResp>(TReq request, string url, Stream? requestStream, CancellationToken token) where TReq : IRequest where TResp : IResponse, new()
    {
        _logger.LogDebug("Sending request to {Url}", url);

        HttpResponse httpResp = await _networkDriver.SendRequestAsync<TResp>(request, url, requestStream, token).ConfigureAwait(false);

        //Clear sensitive material from the request
        if (request is IContainSensitiveMaterial sensitive)
            sensitive.ClearSensitiveMaterial();

        IDictionary<string, string> headers = httpResp.Headers;
        int statusCode = httpResp.StatusCode;

        TResp response = new TResp();
        response.StatusCode = statusCode;
        response.ContentLength = headers.GetHeaderLong(HttpHeaders.ContentLength);
        response.ConnectionClosed = string.Equals("closed", headers.GetOptionalValue(HttpHeaders.Connection), StringComparison.OrdinalIgnoreCase);
        response.Date = headers.GetHeaderDate(HttpHeaders.Date, DateTimeFormat.Rfc1123);
        response.Server = headers.GetOptionalValue(HttpHeaders.Server);
        response.ResponseId = headers.GetOptionalValue(AmzHeaders.XAmzId2);
        response.RequestId = headers.GetOptionalValue(AmzHeaders.XAmzRequestId);

        // https://docs.aws.amazon.com/AmazonS3/latest/API/ErrorResponses.html
        response.IsSuccess = !(statusCode == 403 //Forbidden
                               || statusCode == 400 //BadRequest
                               || statusCode == 500 //InternalServerError
                               || statusCode == 416 //RequestedRangeNotSatisfiable
                               || statusCode == 405 //MethodNotAllowed
                               || statusCode == 411 //LengthRequired
                               || statusCode == 404 //NotFound
                               || statusCode == 501 //NotImplemented
                               || statusCode == 504 //GatewayTimeout
                               || statusCode == 301 //MovedPermanently
                               || statusCode == 412 //PreconditionFailed
                               || statusCode == 307 //TemporaryRedirect
                               || statusCode == 409 //Conflict
                               || statusCode == 503); //ServiceUnavailable

        Stream? responseStream = httpResp.Content;

        //Only marshal successful responses
        if (response.IsSuccess)
            _marshaller.MarshalResponse(_config, response, headers, responseStream ?? Stream.Null);
        else if (responseStream != null)
        {
            try
            {
                using MemoryStream ms = new MemoryStream();
                await responseStream.CopyToAsync(ms, 81920, token).ConfigureAwait(false);

                if (ms.Length > 0)
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    using (responseStream)
                        response.Error = ErrorHandler.Create(ms);

                    _logger.LogDebug("Received error: '{Message}'. Details: '{Details}'", response.Error.Message, response.Error.GetErrorDetails());
                }
            }
            catch
            {
                //Do nothing
            }
        }

        //We always map even if the request is not successful
        _postMapper.PostMap(_config, request, response);

        if (_config.ThrowExceptionOnError && !response.IsSuccess)
            throw new S3RequestException(response, $"Received error: '{response.Error?.Message}'. Details: '{response.Error?.GetErrorDetails()}'");

        return response;
    }
}