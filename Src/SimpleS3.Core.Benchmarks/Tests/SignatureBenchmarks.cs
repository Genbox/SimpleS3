using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[MemoryDiagnoser]
public class SignatureBenchmarks
{
    private readonly ChunkedSignatureBuilder _chunkSigBuilder;
    private readonly DateTimeOffset _date;
    private readonly BaseRequest _req;
    private readonly SignatureBuilder _signatureBuilder;
    private readonly SigningKeyBuilder _signingKeyBuilder;

    public SignatureBenchmarks()
    {
        IAccessKey creds = new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");
        SimpleS3Config config = new SimpleS3Config(creds, "eu1-region");
        config.PayloadSignatureMode = SignatureMode.FullSignature;

        IOptions<SimpleS3Config> options = Options.Create(config);

        _signingKeyBuilder = new SigningKeyBuilder(options, NullLogger<SigningKeyBuilder>.Instance);
        IScopeBuilder scopeBuilder = new ScopeBuilder(options);
        _signatureBuilder = new SignatureBuilder(_signingKeyBuilder, scopeBuilder, options, NullLogger<SignatureBuilder>.Instance);
        _chunkSigBuilder = new ChunkedSignatureBuilder(_signingKeyBuilder, scopeBuilder, NullLogger<ChunkedSignatureBuilder>.Instance);

        byte[] data = "Hello world"u8.ToArray();

        _req = new PutObjectRequest("examplebucket", "benchmark", new MemoryStream(data));
        _req.SetHeader(AmzHeaders.XAmzContentSha256, CryptoHelper.Sha256Hash(data).HexEncode());

        _date = DateTimeOffset.UtcNow;
    }

    [Benchmark]
    public byte[] SignatureKeyBuilder() => _signingKeyBuilder.CreateSigningKey(_date);

    [Benchmark]
    public byte[] SignatureBuilder() => _signatureBuilder.CreateSignature(_req);

    [Benchmark]
    public byte[] ChunkedSignatureBuilder() => _chunkSigBuilder.CreateChunkSignature(_req, new byte[32], new byte[32], 0, 32);
}