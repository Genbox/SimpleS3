using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Builders
{
    public class AuthorizationHeaderBuilder : IAuthorizationBuilder
    {
        private readonly ILogger<AuthorizationHeaderBuilder> _logger;
        private readonly IOptions<S3Config> _options;
        private readonly IScopeBuilder _scopeBuilder;
        private readonly ISignatureBuilder _signatureBuilder;

        public AuthorizationHeaderBuilder(IOptions<S3Config> options, IScopeBuilder scopeBuilder, ISignatureBuilder signatureBuilder, ILogger<AuthorizationHeaderBuilder> logger)
        {
            _options = options;
            _scopeBuilder = scopeBuilder;
            _signatureBuilder = signatureBuilder;
            _logger = logger;
        }

        public string BuildAuthorization(IRequest request)
        {
            Validator.RequireNotNull(request, nameof(request));

            return BuildHeader(request.Date, request.Headers, _signatureBuilder.CreateSignature(request));
        }

        internal string BuildHeader(DateTimeOffset date, IReadOnlyDictionary<string, string> headers, byte[] signature)
        {
            _logger.LogTrace("Building auth header");

            string scope = _scopeBuilder.CreateScope("s3", date);

            StringBuilder header = new StringBuilder(512);
            header.Append(SigningConstants.AlgorithmTag);
            header.AppendFormat(CultureInfo.InvariantCulture, " Credential={0}/{1},", _options.Value.Credentials.KeyId, scope);
            header.AppendFormat(CultureInfo.InvariantCulture, "SignedHeaders={0},", string.Join(";", FilterHeaders(headers)));
            header.AppendFormat(CultureInfo.InvariantCulture, "Signature={0}", signature.HexEncode());

            string authHeader = header.ToString();
            _logger.LogDebug("AuthHeader: {AuthHeader}", authHeader);
            return authHeader;
        }

        private static IEnumerable<string> FilterHeaders(IReadOnlyDictionary<string, string> headers)
        {
            foreach (string key in headers.Select(x => x.Key).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                string loweredKey = key.ToLowerInvariant();

                if (SigningConstants.ShouldSignHeader(loweredKey))
                    yield return loweredKey;
            }
        }
    }
}