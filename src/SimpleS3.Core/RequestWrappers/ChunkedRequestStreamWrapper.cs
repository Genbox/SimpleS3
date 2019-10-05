using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Internal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.RequestWrappers
{
    /// <summary>Add chunked streaming support to requests</summary>
    [PublicAPI]
    public sealed class ChunkedRequestStreamWrapper : IRequestStreamWrapper
    {
        private readonly IChunkedSignatureBuilder _chunkedSigBuilder;
        private readonly IOptions<S3Config> _config;
        private readonly ISignatureBuilder _signatureBuilder;

        public ChunkedRequestStreamWrapper(IOptions<S3Config> config, IChunkedSignatureBuilder chunkedSigBuilder, ISignatureBuilder signatureBuilder)
        {
            Validator.RequireNotNull(config);
            Validator.RequireNotNull(chunkedSigBuilder);
            Validator.RequireNotNull(signatureBuilder);

            _chunkedSigBuilder = chunkedSigBuilder;
            _signatureBuilder = signatureBuilder;
            _config = config;
        }

        public bool IsSupported(IRequest request)
        {
            if (!_config.Value.EnableStreaming)
                return false;

            return request is ISupportStreaming;
        }

        public Stream Wrap(Stream input, IRequest request)
        {
            Validator.RequireNotNull(input);
            Validator.RequireNotNull(request);

            //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
            request.AddHeader(HttpHeaders.ContentEncoding, "aws-chunked");
            request.AddHeader(AmzHeaders.XAmzDecodedContentLength, input.Length);
            request.AddHeader(AmzHeaders.XAmzContentSha256, "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");

            byte[] seedSignature = _signatureBuilder.CreateSignature(request);
            return new ChunkedStream(_config, _chunkedSigBuilder, request, seedSignature, input);
        }
    }
}