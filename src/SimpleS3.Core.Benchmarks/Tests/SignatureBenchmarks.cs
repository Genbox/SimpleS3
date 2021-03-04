using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests
{
    [MemoryDiagnoser]
    [InProcess]
    public class SignatureBenchmarks
    {
        private readonly ChunkedSignatureBuilder _chunkSigBuilder;
        private readonly DateTimeOffset _date;
        private readonly BaseRequest _req;
        private readonly SignatureBuilder _signatureBuilder;
        private readonly ISigningKeyBuilder _signingKeyBuilder;

        public SignatureBenchmarks()
        {
            AmazonS3Config config = new AmazonS3Config(new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY"), AmazonS3Region.UsEast1);
            config.PayloadSignatureMode = SignatureMode.FullSignature;

            IOptions<Config> options = Options.Create(config);

            _signingKeyBuilder = new SigningKeyBuilder(options, NullLogger<SigningKeyBuilder>.Instance);
            IScopeBuilder scopeBuilder = new ScopeBuilder(options);
            AmazonS3UrlBuilder urlBuilder = new AmazonS3UrlBuilder(options);
            _signatureBuilder = new SignatureBuilder(_signingKeyBuilder, scopeBuilder, urlBuilder, NullLogger<SignatureBuilder>.Instance);
            _chunkSigBuilder = new ChunkedSignatureBuilder(_signingKeyBuilder, scopeBuilder, NullLogger<ChunkedSignatureBuilder>.Instance);

            byte[] data = Encoding.UTF8.GetBytes("Hello world");

            _req = new PutObjectRequest("examplebucket", "benchmark", new MemoryStream(data));
            _req.SetHeader(AmzHeaders.XAmzContentSha256, CryptoHelper.Sha256Hash(data).HexEncode());

            _date = DateTimeOffset.UtcNow;
        }

        [Benchmark]
        public byte[] SignatureKeyBuilder()
        {
            return _signingKeyBuilder.CreateSigningKey(_date);
        }

        [Benchmark]
        public byte[] SignatureBuilder()
        {
            return _signatureBuilder.CreateSignature(_req);
        }

        [Benchmark]
        public byte[] ChunkedSignatureBuilder()
        {
            return _chunkSigBuilder.CreateChunkSignature(_req, new byte[32], new byte[32], 0, 32);
        }
    }
}