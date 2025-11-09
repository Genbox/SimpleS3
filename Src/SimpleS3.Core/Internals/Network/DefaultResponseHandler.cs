using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Errors;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

public class DefaultResponseHandler : IResponseHandler
{
    private readonly SimpleS3Config _config;
    private readonly ILogger<DefaultResponseHandler> _logger;
    private readonly IMarshalFactory _marshaller;
    private readonly INetworkDriver _networkDriver;
    private readonly IPostMapperFactory _postMapper;

    public DefaultResponseHandler(IOptions<SimpleS3Config> options, IRequestValidatorFactory validator, IMarshalFactory marshaller, IPostMapperFactory postMapper, INetworkDriver networkDriver, ILogger<DefaultResponseHandler> logger)
    {
        Validator.RequireNotNull(options);
        Validator.RequireNotNull(validator);
        Validator.RequireNotNull(marshaller);
        Validator.RequireNotNull(networkDriver);
        Validator.RequireNotNull(logger);

        _networkDriver = networkDriver;
        _postMapper = postMapper;
        _logger = logger;
        _marshaller = marshaller;
        _config = options.Value;
    }

    public async Task<TResp> HandleResponseAsync<TReq, TResp>(TReq request, string url, Stream? requestStream, CancellationToken token) where TReq : IRequest where TResp : IResponse, new()
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

        ContentStream? responseStream = httpResp.Content;

        //Only marshal successful responses
        if (response.IsSuccess)
            _marshaller.MarshalResponse(_config, response, headers, responseStream ?? ContentStream.Null);
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
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to map error");
            }
        }

        //We always map even if the request is not successful
        _postMapper.PostMap(_config, request, response);

        if (_config.ThrowExceptionOnError && !response.IsSuccess)
            throw new S3RequestException(response, $"Received error: '{response.Error?.Message}'. Details: '{response.Error?.GetErrorDetails()}'");

        return response;
    }
}