using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal class DefaultSignedRequestHandler : ISignedRequestHandler
{
    private readonly IAuthorizationBuilder _authBuilder;
    private readonly IEndpointBuilder _endpointBuilder;
    private readonly ILogger<DefaultSignedRequestHandler> _logger;
    private readonly IMarshalFactory _marshaller;
    private readonly IOptions<SimpleS3Config> _options;
    private readonly IScopeBuilder _scopeBuilder;

    public DefaultSignedRequestHandler(IOptions<SimpleS3Config> options, IScopeBuilder scopeBuilder, IMarshalFactory marshaller, QueryParameterAuthorizationBuilder authBuilder, IEndpointBuilder endpointBuilder, ILogger<DefaultSignedRequestHandler> logger)
    {
        Validator.RequireNotNull(options, nameof(options));
        Validator.RequireNotNull(marshaller, nameof(marshaller));
        Validator.RequireNotNull(authBuilder, nameof(authBuilder));
        Validator.RequireNotNull(logger, nameof(logger));

        _options = options;
        _authBuilder = authBuilder;
        _endpointBuilder = endpointBuilder;
        _marshaller = marshaller;
        _logger = logger;
        _scopeBuilder = scopeBuilder;
    }

    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest
    {
        request.Timestamp = DateTimeOffset.UtcNow;
        request.RequestId = Guid.NewGuid();

        _logger.LogTrace("Handling {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

        SimpleS3Config config = _options.Value;
        _marshaller.MarshalRequest(config, request);

        IEndpointData endpointData = _endpointBuilder.GetEndpoint(request);
        request.SetHeader(HttpHeaders.Host, endpointData.Host);

        string scope = _scopeBuilder.CreateScope("s3", request.Timestamp);
        request.SetQueryParameter(AmzParameters.XAmzAlgorithm, SigningConstants.AlgorithmTag);
        request.SetQueryParameter(AmzParameters.XAmzCredential, _options.Value.Credentials.KeyId + '/' + scope);
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
        RequestHelper.AppendQueryParameters(sb, request);
        return StringBuilderPool.Shared.ReturnString(sb);
    }
}