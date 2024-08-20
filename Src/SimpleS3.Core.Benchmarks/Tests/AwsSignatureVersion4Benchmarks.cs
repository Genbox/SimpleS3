using Amazon.Runtime;
using Amazon.Util;
using AwsSignatureVersion4.Private;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Benchmarks.Misc;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

//This benchmark tests against https://github.com/FantasticFiasco/aws-signature-version-4

[MemoryDiagnoser]
public sealed class AwsSignatureVersion4Benchmarks : IDisposable
{
    private HeaderAuthorizationBuilder _builder = null!;
    private ImmutableCredentials _credentials = null!;
    private DummyRequest _request = null!;
    private HttpRequestMessage _request2 = null!;

    public void Dispose() => _request2.Dispose();

    [GlobalSetup]
    public void Setup()
    {
        {
            IAccessKey credentials = new StringAccessKey("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123");
            SimpleS3Config config = new SimpleS3Config(credentials, "eu1-region");

            IOptions<SimpleS3Config> options = Options.Create(config);

            SigningKeyBuilder signingKeyBuilder = new SigningKeyBuilder(options, NullLogger<SigningKeyBuilder>.Instance);
            ScopeBuilder scopeBuilder = new ScopeBuilder(options);
            SignatureBuilder signatureBuilder = new SignatureBuilder(signingKeyBuilder, scopeBuilder, options, NullLogger<SignatureBuilder>.Instance);

            _builder = new HeaderAuthorizationBuilder(options, scopeBuilder, signatureBuilder, NullLogger<HeaderAuthorizationBuilder>.Instance);

            _request = new DummyRequest();
            _request.SetHeader(AmzHeaders.XAmzContentSha256, "UNSIGNED-PAYLOAD");
        }

        {
            _request2 = new HttpRequestMessage(HttpMethod.Get, "https://dummyurl");
            _credentials = new ImmutableCredentials("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123", null);

            // Add required headers
            _request2.AddHeader(HeaderKeys.XAmzDateHeader, DateTime.UtcNow.ToIso8601BasicDateTime());

            // Add conditional headers
            _request2.AddHeaderIf(_credentials.UseToken, HeaderKeys.XAmzSecurityTokenHeader, _credentials.Token);
            _request2.AddHeaderIf(!_request2.Headers.Contains(HeaderKeys.HostHeader), HeaderKeys.HostHeader, _request2.RequestUri?.Host!);
        }
    }

    [Benchmark]
    public void SimpleS3()
    {
        _builder.BuildAuthorization(_request);
    }

    [Benchmark]
    public string Aws4()
    {
        // Build the canonical request
        (string canonicalRequest, string signedHeaders) = CanonicalRequest.Build("s3", _request2, new Dictionary<string, IEnumerable<string>>(), string.Empty);

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