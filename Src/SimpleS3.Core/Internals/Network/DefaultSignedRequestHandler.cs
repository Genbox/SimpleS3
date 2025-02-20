using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal sealed class DefaultSignedRequestHandler : ISignedRequestHandler
{
    private readonly QueryParameterAuthorizationBuilder _authBuilder;
    private readonly SimpleS3Config _config;
    private readonly IEndpointBuilder _endpointBuilder;
    private readonly ILogger<DefaultSignedRequestHandler> _logger;
    private readonly IMarshalFactory _marshaller;
    private readonly IResponseHandler _responseHandler;
    private readonly IScopeBuilder _scopeBuilder;

    public DefaultSignedRequestHandler(IOptions<SimpleS3Config> options, IMarshalFactory marshaller, IScopeBuilder scopeBuilder, QueryParameterAuthorizationBuilder authBuilder, IEndpointBuilder endpointBuilder, IResponseHandler responseHandler, ILogger<DefaultSignedRequestHandler> logger)
    {
        Validator.RequireNotNull(options);
        Validator.RequireNotNull(authBuilder);
        Validator.RequireNotNull(logger);

        _config = options.Value;
        _marshaller = marshaller;
        _authBuilder = authBuilder;
        _endpointBuilder = endpointBuilder;
        _responseHandler = responseHandler;
        _logger = logger;
        _scopeBuilder = scopeBuilder;
    }

    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest
    {
        if (_config.Credentials == null)
            throw new InvalidOperationException("You cannot pre-sign requests without first providing credentials");

        request.Timestamp = DateTimeOffset.UtcNow;
        request.RequestId = Guid.NewGuid();

        _marshaller.MarshalRequest(_config, request); //We don't use the return stream

        _logger.LogTrace("Handling {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

        IEndpointData endpointData = _endpointBuilder.GetEndpoint(request);
        request.SetHeader(HttpHeaders.Host, endpointData.Host);

        string scope = _scopeBuilder.CreateScope("s3", request.Timestamp);
        request.SetQueryParameter(AmzParameters.XAmzAlgorithm, SigningConstants.AlgorithmTag);
        request.SetQueryParameter(AmzParameters.XAmzCredential, _config.Credentials.KeyId + '/' + scope);
        request.SetQueryParameter(AmzParameters.XAmzDate, request.Timestamp.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
        request.SetQueryParameter(AmzParameters.XAmzExpires, expiresIn.TotalSeconds.ToString(NumberFormatInfo.InvariantInfo));
        request.SetQueryParameter(AmzParameters.XAmzSignedHeaders, string.Join(";", HeaderWhitelist.FilterHeaders(request.Headers).Select(x => x.Key)));

        //Copy all headers to query parameters
        foreach (KeyValuePair<string, string> header in request.Headers)
        {
            if (header.Key == HttpHeaders.Host)
                continue;

            request.SetQueryParameter(header.Key, header.Value);
        }

        _authBuilder.BuildAuthorization(request);

        //Clear sensitive material from the request
        if (request is IContainSensitiveMaterial sensitive)
            sensitive.ClearSensitiveMaterial();

        StringBuilder sb = StringBuilderPool.Shared.Rent(200);
        sb.Append(endpointData.Endpoint);
        RequestHelper.AppendPath(sb, _config, request);
        RequestHelper.AppendQueryParameters(sb, request);
        return StringBuilderPool.Shared.ReturnString(sb);
    }

    public async Task<TResp> SendRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new()
    {
        SignedRequest req = new SignedRequest(httpMethod);
        return await _responseHandler.HandleResponseAsync<SignedRequest, TResp>(req, url, content, token).ConfigureAwait(false);
    }
}