using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Builders
{
    public class QueryParameterAuthorizationBuilder : IAuthorizationBuilder
    {
        private readonly ILogger<QueryParameterAuthorizationBuilder> _logger;
        private readonly ISignatureBuilder _signatureBuilder;

        public QueryParameterAuthorizationBuilder(ISignatureBuilder signatureBuilder, ILogger<QueryParameterAuthorizationBuilder> logger)
        {
            _signatureBuilder = signatureBuilder;
            _logger = logger;
        }

        public void BuildAuthorization(IRequest request)
        {
            Validator.RequireNotNull(request, nameof(request));

            _logger.LogTrace("Building parameter based authorization");
            request.SetQueryParameter(AmzParameters.XAmzSignature, _signatureBuilder.CreateSignature(request, false).HexEncode());
        }
    }
}