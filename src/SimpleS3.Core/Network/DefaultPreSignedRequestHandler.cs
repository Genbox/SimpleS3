using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Internals.Constants;
using Genbox.SimpleS3.Core.Internals.Pools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network
{
    public class DefaultPreSignedRequestHandler : IPreSignRequestHandler
    {
        private readonly IAuthorizationBuilder _authBuilder;
        private readonly ILogger<DefaultPreSignedRequestHandler> _logger;
        private readonly IMarshalFactory _marshaller;
        private readonly IOptions<S3Config> _options;
        private readonly IScopeBuilder _scopeBuilder;
        private readonly IValidatorFactory _validator;

        public DefaultPreSignedRequestHandler(IOptions<S3Config> options, IScopeBuilder scopeBuilder, IValidatorFactory validator, IMarshalFactory marshaller, QueryParameterAuthorizationBuilder authBuilder, ILogger<DefaultPreSignedRequestHandler> logger)
        {
            Validator.RequireNotNull(options, nameof(options));
            Validator.RequireNotNull(validator, nameof(validator));
            Validator.RequireNotNull(marshaller, nameof(marshaller));
            Validator.RequireNotNull(authBuilder, nameof(authBuilder));
            Validator.RequireNotNull(logger, nameof(logger));

            validator.ValidateAndThrow(options.Value);

            _validator = validator;
            _options = options;
            _authBuilder = authBuilder;
            _marshaller = marshaller;
            _logger = logger;
            _scopeBuilder = scopeBuilder;
        }

        public Task<string> SignRequestAsync<TReq>(TReq request, TimeSpan expiresIn, CancellationToken cancellationToken = default) where TReq : IRequest
        {
            cancellationToken.ThrowIfCancellationRequested();

            request.Timestamp = DateTimeOffset.UtcNow;
            request.RequestId = Guid.NewGuid();

            _logger.LogTrace("Handling {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

            S3Config config = _options.Value;
            _marshaller.MarshalRequest(request, config);

            _validator.ValidateAndThrow(request);

            StringBuilder sb = StringBuilderPool.Shared.Rent(200);
            RequestHelper.AppendHost(sb, config, request);

            string host = sb.ToString();
            request.SetHeader(HttpHeaders.Host, host);

            string scope = _scopeBuilder.CreateScope("s3", request.Timestamp);
            request.SetQueryParameter(AmzParameters.XAmzAlgorithm, SigningConstants.AlgorithmTag);
            request.SetQueryParameter(AmzParameters.XAmzCredential, _options.Value.Credentials.KeyId + '/' + scope);
            request.SetQueryParameter(AmzParameters.XAmzDate, request.Timestamp.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
            request.SetQueryParameter(AmzParameters.XAmzExpires, expiresIn.TotalSeconds.ToString(NumberFormatInfo.InvariantInfo));
            request.SetQueryParameter(AmzParameters.XAmzSignedHeaders, string.Join(";", SigningConstants.FilterHeaders(request.Headers).Select(x => x.Key)));

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

            //We need to clear the StringBuilder as we need ot prepend the scheme
            sb.Clear();

            RequestHelper.AppendScheme(sb, config);
            sb.Append(host);
            RequestHelper.AppendUrl(sb, config, request);
            RequestHelper.AppendQueryParameters(sb, request);

            string url = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return Task.FromResult(url);
        }
    }
}