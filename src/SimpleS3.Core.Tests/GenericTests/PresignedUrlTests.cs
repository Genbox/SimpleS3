using System;
using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Internals.Constants;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests
{
    public class PreSignedUrlTests
    {
        private readonly SignatureBuilder _sigBuilder;
        private readonly ScopeBuilder _scopeBuilder;
        private readonly DateTimeOffset _testDate = new DateTimeOffset(2013, 05, 24, 0, 0, 0, TimeSpan.Zero);
        private readonly IOptions<S3Config> _options;

        public PreSignedUrlTests()
        {
            //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-query-string-auth.html
            S3Config config = new S3Config(new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY"), AwsRegion.UsEast1);

            _options = Options.Create(config);

            SigningKeyBuilder keyBuilder = new SigningKeyBuilder(_options, NullLogger<SigningKeyBuilder>.Instance);
            _scopeBuilder = new ScopeBuilder(_options);
            _sigBuilder = new SignatureBuilder(keyBuilder, _scopeBuilder, NullLogger<SignatureBuilder>.Instance, _options);
        }

        [Fact]
        public void TestAgainstAmazonsExample()
        {
            string scope = _scopeBuilder.CreateScope("s3", _testDate);

            GetObjectRequest request = new GetObjectRequest("examplebucket", "test.txt");
            request.SetHeader("host", "examplebucket.s3.amazonaws.com");
            request.SetQueryParameter(AmzParameters.XAmzAlgorithm, SigningConstants.AlgorithmTag);
            request.SetQueryParameter(AmzParameters.XAmzCredential, _options.Value.Credentials.KeyId + '/' + scope);
            request.SetQueryParameter(AmzParameters.XAmzDate, _testDate.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
            request.SetQueryParameter(AmzParameters.XAmzExpires, "86400");
            request.SetQueryParameter(AmzParameters.XAmzSignedHeaders, "host");

            string canonicalRequest = _sigBuilder.CreateCanonicalRequest(request.RequestId, '/' + request.ObjectKey, request.Method, request.Headers, request.QueryParameters, "UNSIGNED-PAYLOAD");

            string expectedCanonicalRequest = "GET" + SigningConstants.Newline +
                                              "/test.txt" + SigningConstants.Newline +
                                              "X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAIOSFODNN7EXAMPLE%2F20130524%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20130524T000000Z&X-Amz-Expires=86400&X-Amz-SignedHeaders=host" + SigningConstants.Newline +
                                              "host:examplebucket.s3.amazonaws.com" + SigningConstants.Newline +
                                              "" + SigningConstants.Newline +
                                              "host" + SigningConstants.Newline +
                                              "UNSIGNED-PAYLOAD";

            Assert.Equal(expectedCanonicalRequest, canonicalRequest);

            string stringToSign = _sigBuilder.CreateStringToSign(_testDate, scope, canonicalRequest);

            string expectedStringtoSign = "AWS4-HMAC-SHA256" + SigningConstants.Newline +
                                          "20130524T000000Z" + SigningConstants.Newline +
                                          "20130524/us-east-1/s3/aws4_request" + SigningConstants.Newline +
                                          "3bfa292879f6447bbcda7001decf97f4a54dc650c8942174ae0a9121cf58ad04";

            Assert.Equal(expectedStringtoSign, stringToSign);

            byte[] signature = _sigBuilder.CreateSignature(_testDate, stringToSign);

            Assert.Equal("aeeed9bbccd4d02ee5c0109b86d86835f995330da4c265957d157751f604d404", signature.HexEncode());
        }
    }
}
