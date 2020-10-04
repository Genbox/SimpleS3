using System;
using System.Net.Http;
using Amazon.Runtime;
using Amazon.Util;
using AwsSignatureVersion4.Private;
using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Network;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    //This benchmark tests against https://github.com/FantasticFiasco/aws-signature-version-4

    [MemoryDiagnoser]
    [InProcess]
    public class AwsSignatureVersion4Benchmarks
    {
        private HeaderAuthorizationBuilder _builder;
        private ImmutableCredentials _credentials;
        private DummyRequest _request;
        private HttpRequestMessage _request2;

        [GlobalSetup]
        public void Setup()
        {
            {
                AwsConfig config = new AwsConfig();
                config.Region = AwsRegion.EuWest1;
                config.Credentials = new StringAccessKey("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123");

                IOptions<AwsConfig> options = Options.Create(config);

                SigningKeyBuilder signingKeyBuilder = new SigningKeyBuilder(options, NullLogger<SigningKeyBuilder>.Instance);
                ScopeBuilder scopeBuilder = new ScopeBuilder(options);
                AwsUrlBuilder urlBuilder = new AwsUrlBuilder(options);
                SignatureBuilder signatureBuilder = new SignatureBuilder(signingKeyBuilder, scopeBuilder, urlBuilder, NullLogger<SignatureBuilder>.Instance, options);

                _builder = new HeaderAuthorizationBuilder(options, scopeBuilder, signatureBuilder, NullLogger<HeaderAuthorizationBuilder>.Instance);

                _request = new DummyRequest();
                _request.SetHeader(AmzHeaders.XAmzContentSha256, "UNSIGNED-PAYLOAD");
            }

            {
                _request2 = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, "https://dummyurl");
                _credentials = new ImmutableCredentials("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123", null);

                // Add required headers
                _request2.AddHeader(HeaderKeys.XAmzDateHeader, DateTime.UtcNow.ToIso8601BasicDateTime());

                // Add conditional headers
                _request2.AddHeaderIf(_credentials.UseToken, HeaderKeys.XAmzSecurityTokenHeader, _credentials.Token);
                _request2.AddHeaderIf(!_request2.Headers.Contains(HeaderKeys.HostHeader), HeaderKeys.HostHeader, _request2.RequestUri.Host);
            }
        }

        [Benchmark]
        public void SimpleS3()
        {
            _builder.BuildAuthorization(_request);
        }

        [Benchmark]
        public string ASV4()
        {
            // Build the canonical request
            (string canonicalRequest, string signedHeaders) = CanonicalRequest.Build("s3", _request2, null, null);

            // Build the string to sign
            (string stringToSign, string credentialScope) = StringToSign.Build(
                DateTime.UtcNow,
                "eu-west-1",
                "S3",
                canonicalRequest);

            // Build the authorization header
            string authorizationHeader = AuthorizationHeader.Build(
                DateTime.UtcNow,
                "eu-west-1",
                "S3",
                _credentials,
                signedHeaders,
                credentialScope,
                stringToSign);

            return authorizationHeader;
        }
    }
}