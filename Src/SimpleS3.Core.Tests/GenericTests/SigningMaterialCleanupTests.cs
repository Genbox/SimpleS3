using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class SigningMaterialCleanupTests
{
    [Fact]
    public void SignatureBuilderClearsSigningKey()
    {
        TrackingSigningKeyBuilder keyBuilder = new TrackingSigningKeyBuilder();
        SignatureBuilder builder = new SignatureBuilder(keyBuilder, new TestScopeBuilder(), Options.Create(CreateConfig()), NullLogger<SignatureBuilder>.Instance);

        byte[] signature = builder.CreateSignature(DateTimeOffset.UtcNow, "string-to-sign");

        Assert.NotEmpty(signature);
        Assert.All(keyBuilder.SigningKey, x => Assert.Equal(0, x));
    }

    [Fact]
    public void ChunkedSignatureBuilderClearsSigningKey()
    {
        TrackingSigningKeyBuilder keyBuilder = new TrackingSigningKeyBuilder();
        ChunkedSignatureBuilder builder = new ChunkedSignatureBuilder(keyBuilder, new TestScopeBuilder(), Options.Create(CreateConfig()), NullLogger<ChunkedSignatureBuilder>.Instance);

        byte[] signature = builder.CreateSignature(DateTimeOffset.UtcNow, "string-to-sign");

        Assert.NotEmpty(signature);
        Assert.All(keyBuilder.SigningKey, x => Assert.Equal(0, x));
    }

    [Fact]
    public void HeaderAuthorizationBuilderClearsSignature()
    {
        TrackingSignatureBuilder signatureBuilder = new TrackingSignatureBuilder();
        HeaderAuthorizationBuilder builder = new HeaderAuthorizationBuilder(Options.Create(CreateConfig()), new TestScopeBuilder(), signatureBuilder, NullLogger<HeaderAuthorizationBuilder>.Instance);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null)
        {
            Timestamp = DateTimeOffset.UtcNow
        };
        request.SetHeader(HttpHeaders.Host, "example.com");

        builder.BuildAuthorization(request);

        Assert.All(signatureBuilder.Signature, x => Assert.Equal(0, x));
    }

    [Fact]
    public void QueryParameterAuthorizationBuilderClearsSignature()
    {
        TrackingSignatureBuilder signatureBuilder = new TrackingSignatureBuilder();
        QueryParameterAuthorizationBuilder builder = new QueryParameterAuthorizationBuilder(signatureBuilder, NullLogger<QueryParameterAuthorizationBuilder>.Instance);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null);

        builder.BuildAuthorization(request);

        Assert.All(signatureBuilder.Signature, x => Assert.Equal(0, x));
    }

    private static SimpleS3Config CreateConfig() => new SimpleS3Config
    {
        Credentials = new StringAccessKey("key-id", "secret-key"),
        RegionCode = "us-east-1"
    };

    private sealed class TrackingSigningKeyBuilder : ISigningKeyBuilder
    {
        public byte[] SigningKey { get; } = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];

        public byte[] CreateSigningKey(DateTimeOffset dateTime) => SigningKey;
    }

    private sealed class TrackingSignatureBuilder : ISignatureBuilder
    {
        public byte[] Signature { get; } = [1, 2, 3, 4, 5, 6, 7, 8];

        public byte[] CreateSignature(IRequest request, bool enablePayloadSignature = true) => Signature;
    }

    private sealed class TestScopeBuilder : IScopeBuilder
    {
        public string CreateScope(string service, DateTimeOffset date) => "20260524/us-east-1/" + service + "/aws4_request";
    }
}