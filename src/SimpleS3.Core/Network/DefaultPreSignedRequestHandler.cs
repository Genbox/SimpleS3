using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common;
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
        private readonly IValidatorFactory _validator;

        public DefaultPreSignedRequestHandler(IOptions<S3Config> options, IValidatorFactory validator, IMarshalFactory marshaller, IAuthorizationBuilder authBuilder, ILogger<DefaultPreSignedRequestHandler> logger)
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
        }

        public Task<string> SignRequestAsync<TReq>(TReq request, CancellationToken cancellationToken = default) where TReq : IRequest
        {
            cancellationToken.ThrowIfCancellationRequested();

            request.Timestamp = DateTimeOffset.UtcNow;
            request.RequestId = Guid.NewGuid();

            _logger.LogTrace("Handling {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

            S3Config config = _options.Value;
            _marshaller.MarshalRequest(request, config);

            _validator.ValidateAndThrow(request);

            (string? host, string? url) = RequestHelper.BuildEndpointData(config, request);

            request.SetHeader(HttpHeaders.Host, host);
            request.SetHeader(AmzHeaders.XAmzContentSha256, "UNSIGNED-PAYLOAD");

            string authorization = url + "&" + _authBuilder.BuildAuthorization(request);

            //Clear sensitive material from the request
            if (request is IContainSensitiveMaterial sensitive)
                sensitive.ClearSensitiveMaterial();

            return Task.FromResult(authorization);
        }
    }
}