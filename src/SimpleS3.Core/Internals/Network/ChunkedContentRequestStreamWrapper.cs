using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

/// <summary>Add chunked streaming support to requests</summary>
internal sealed class ChunkedContentRequestStreamWrapper : IRequestStreamWrapper
{
    private readonly IChunkedSignatureBuilder _chunkedSigBuilder;
    private readonly SimpleS3Config _config;
    private readonly ISignatureBuilder _signatureBuilder;

    public ChunkedContentRequestStreamWrapper(IOptions<SimpleS3Config> config, IChunkedSignatureBuilder chunkedSigBuilder, ISignatureBuilder signatureBuilder)
    {
        Validator.RequireNotNull(config, nameof(config));
        Validator.RequireNotNull(chunkedSigBuilder, nameof(chunkedSigBuilder));
        Validator.RequireNotNull(signatureBuilder, nameof(signatureBuilder));

        _chunkedSigBuilder = chunkedSigBuilder;
        _signatureBuilder = signatureBuilder;
        _config = config.Value;
    }

    public bool IsSupported(IRequest request)
    {
        return _config.PayloadSignatureMode == SignatureMode.StreamingSignature && request is ISupportStreaming;
    }

    public Stream Wrap(Stream input, IRequest request)
    {
        //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
        request.SetHeader(HttpHeaders.ContentEncoding, "aws-chunked");
        request.SetHeader(AmzHeaders.XAmzDecodedContentLength, input.Length);
        request.SetHeader(AmzHeaders.XAmzContentSha256, "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");

        byte[] seedSignature = _signatureBuilder.CreateSignature(request);
        return new ChunkedStream(_config, _chunkedSigBuilder, request, seedSignature, input);
    }
}