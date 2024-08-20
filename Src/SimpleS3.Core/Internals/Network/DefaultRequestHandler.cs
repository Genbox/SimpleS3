using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

/// <summary>Handles common request and response logic before sending to transport drivers.</summary>
internal class DefaultRequestHandler : IRequestHandler
{
    private readonly HeaderAuthorizationBuilder _authBuilder;
    private readonly SimpleS3Config _config;
    private readonly IEndpointBuilder _endpointBuilder;
    private readonly ILogger<DefaultRequestHandler> _logger;
    private readonly IMarshalFactory _marshaller;
    private readonly IRequestStreamWrapper[]? _requestStreamWrappers;
    private readonly IRequestValidatorFactory _requestValidator;
    private readonly IResponseHandler _responseHandler;

    public DefaultRequestHandler(IOptions<SimpleS3Config> options, IRequestValidatorFactory validator, IMarshalFactory marshaller, IResponseHandler responseHandler, HeaderAuthorizationBuilder authBuilder, IEndpointBuilder endpointBuilder, ILogger<DefaultRequestHandler> logger, IEnumerable<IRequestStreamWrapper>? requestStreamWrappers = null)
    {
        Validator.RequireNotNull(options);
        Validator.RequireNotNull(validator);
        Validator.RequireNotNull(marshaller);
        Validator.RequireNotNull(responseHandler);
        Validator.RequireNotNull(authBuilder);
        Validator.RequireNotNull(logger);

        _requestValidator = validator;
        _config = options.Value;
        _responseHandler = responseHandler;
        _authBuilder = authBuilder;
        _endpointBuilder = endpointBuilder;
        _marshaller = marshaller;
        _logger = logger;

        _requestStreamWrappers = requestStreamWrappers?.ToArray();
    }

    public Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken token = default) where TReq : IRequest where TResp : IResponse, new()
    {
        token.ThrowIfCancellationRequested();

        request.Timestamp = DateTimeOffset.UtcNow;
        request.RequestId = Guid.NewGuid();

        _logger.LogTrace("Handling {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

        Stream? requestStream = _marshaller.MarshalRequest(_config, request);

        _requestValidator.ValidateAndThrow(request);

        IEndpointData endpointData = _endpointBuilder.GetEndpoint(request);
        request.SetHeader(HttpHeaders.Host, endpointData.Host);
        request.SetHeader(AmzHeaders.XAmzDate, request.Timestamp, DateTimeFormat.Iso8601DateTime);

        if (requestStream != null && _requestStreamWrappers != null)
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

        return _responseHandler.HandleResponseAsync<TReq, TResp>(request, url, requestStream, token);
    }
}