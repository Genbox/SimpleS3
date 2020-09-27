using System;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests
{
    public class PresignedUrlTests
    {
        private readonly SignatureBuilder _sigBuilder;
        private readonly ScopeBuilder _scopeBuilder;
        private readonly DateTimeOffset _testDate = new DateTimeOffset(2013, 05, 24, 0, 0, 0, TimeSpan.Zero);
        private readonly QueryParameterAuthorizationBuilder _authBuilder;
        private readonly IOptions<S3Config> _options;

        public PresignedUrlTests()
        {
            //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-query-string-auth.html
            S3Config config = new S3Config(new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY"), AwsRegion.UsEast1);

            _options = Options.Create(config);

            SigningKeyBuilder keyBuilder = new SigningKeyBuilder(_options, NullLogger<SigningKeyBuilder>.Instance);
            _scopeBuilder = new ScopeBuilder(_options);
            _sigBuilder = new SignatureBuilder(keyBuilder, _scopeBuilder, NullLogger<SignatureBuilder>.Instance, _options);
            _authBuilder = new QueryParameterAuthorizationBuilder(_options, _scopeBuilder, _sigBuilder, NullLogger<QueryParameterAuthorizationBuilder>.Instance);
        }

        [Fact]
        public void TestAgainstAmazonsExample()
        {
            string scope = _scopeBuilder.CreateScope("s3", _testDate);

            GetObjectRequest request = new GetObjectRequest("examplebucket", "test.txt");
            request.SetHeader(AmzHeaders.XAmzContentSha256, "UNSIGNED-PAYLOAD");
            request.SetHeader("host", "examplebucket.s3.amazonaws.com");
            //request.SetHeader("X-Amz-Algorithm", "AWS4-HMAC-SHA256");
            //request.SetHeader("X-Amz-Credential", scope);
            //request.SetHeader("X-Amz-Date", _testDate.ToString(DateTimeFormats.Iso8601Date));
            //request.SetHeader("X-Amz-Expires", "1000");

            string canonicalRequest = _sigBuilder.CreateCanonicalRequest(request.RequestId, request.ObjectKey, request.Method, request.Headers, request.QueryParameters, "UNSIGNED-PAYLOAD");
            string stringToSign = _sigBuilder.CreateStringToSign(_testDate, scope, canonicalRequest);

            byte[] signature = _sigBuilder.CreateSignature(_testDate, stringToSign);

            Assert.Equal("aeeed9bbccd4d02ee5c0109b86d86835f995330da4c265957d157751f604d404", signature.HexEncode());

            //TODO: Perhaps reverse the responsibility so that the authbuilders manipulate the request? Otherwise I'd have to parse the url here to add it to the full request url
            string? queryUrl = _authBuilder.BuildInternal(_testDate, request.Headers, signature);

        }
    }
}
