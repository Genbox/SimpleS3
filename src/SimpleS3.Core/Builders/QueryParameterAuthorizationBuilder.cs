using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Internals.Constants;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Builders
{
    public class QueryParameterAuthorizationBuilder : IAuthorizationBuilder
    {
        private readonly ILogger<QueryParameterAuthorizationBuilder> _logger;
        private readonly IOptions<S3Config> _options;
        private readonly IScopeBuilder _scopeBuilder;
        private readonly ISignatureBuilder _signatureBuilder;

        public QueryParameterAuthorizationBuilder(IOptions<S3Config> options, IScopeBuilder scopeBuilder, ISignatureBuilder signatureBuilder, ILogger<QueryParameterAuthorizationBuilder> logger)
        {
            _options = options;
            _scopeBuilder = scopeBuilder;
            _signatureBuilder = signatureBuilder;
            _logger = logger;
        }

        public string BuildAuthorization(IRequest request)
        {
            Validator.RequireNotNull(request, nameof(request));

            return BuildInternal(request.Timestamp, request.Headers, _signatureBuilder.CreateSignature(request));
        }

        internal string BuildInternal(DateTimeOffset date, IReadOnlyDictionary<string, string> headers, byte[] signature)
        {
            _logger.LogTrace("Building parameter based authorization");

            string scope = _scopeBuilder.CreateScope("s3", date);

            List<KeyValuePair<string, string>> paramBuilder = new List<KeyValuePair<string, string>>(5);
            paramBuilder.Add(new KeyValuePair<string, string>(AmzHeaders.XAmzAlgorithm, SigningConstants.AlgorithmTag));
            paramBuilder.Add(new KeyValuePair<string, string>(AmzHeaders.XAmzCredential, _options.Value.Credentials.KeyId + '/' + scope));
            paramBuilder.Add(new KeyValuePair<string, string>(AmzHeaders.XAmzDate, date.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo)));
            paramBuilder.Add(new KeyValuePair<string, string>(AmzHeaders.XAmzSignedHeaders, string.Join(";", SigningConstants.FilterHeaders(headers).Select(x => x.Key))));
            paramBuilder.Add(new KeyValuePair<string, string>(AmzHeaders.XAmzSignature, signature.HexEncode()));

            string parameters = UrlHelper.CreateQueryString(paramBuilder);
            _logger.LogDebug("Auth parameters: {AuthParameters}", parameters);
            return parameters;
        }
    }
}