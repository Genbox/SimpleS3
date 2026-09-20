using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
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
        Validator.RequireNotNull(config);
        Validator.RequireNotNull(chunkedSigBuilder);
        Validator.RequireNotNull(signatureBuilder);

        _chunkedSigBuilder = chunkedSigBuilder;
        _signatureBuilder = signatureBuilder;
        _config = config.Value;
    }

    private static IReadOnlyDictionary<string, string> EmptyTrailers { get; } = new Dictionary<string, string>(0, StringComparer.Ordinal);

    public bool IsSupported(IRequest request) => _config.PayloadSignatureMode == SignatureMode.StreamingSignature && request is ISupportStreaming && _config.Credentials != null;

    public Stream Wrap(Stream input, IRequest request)
    {
        //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
        long decodedContentLength;

        try
        {
            decodedContentLength = input.Length - (input.CanSeek ? input.Position : 0);
        }
        catch (NotSupportedException ex)
        {
            throw new NotSupportedException("Streaming signature uploads require a stream that exposes Length.", ex);
        }

        if (decodedContentLength < 0)
            throw new InvalidOperationException("The stream position cannot be greater than its length.");

        request.SetHeader(HttpHeaders.ContentEncoding, GetContentEncoding(request));
        request.SetHeader(AmzHeaders.XAmzDecodedContentLength, decodedContentLength);
        IRequestTrailers? requestTrailers = request as IRequestTrailers;
        ChecksumAlgorithm trailingChecksum = ConfigureTrailingChecksum(request, requestTrailers);
        IReadOnlyDictionary<string, string> trailers = requestTrailers?.Trailers ?? EmptyTrailers;
        request.SetHeader(HttpHeaders.ContentLength, ChunkedStream.ComputeChunkedContentLength(decodedContentLength, _config.StreamingChunkSize, ChunkedStream.SignatureLength, trailers, trailingChecksum));

        if (trailers.Count > 0)
        {
            request.SetHeader(AmzHeaders.XAmzTrailer, string.Join(",", trailers.Keys.OrderBy(x => x, StringComparer.Ordinal)));
            request.SetHeader(AmzHeaders.XAmzContentSha256, SigningConstants.StreamingPayloadTrailer);
        }
        else
            request.SetHeader(AmzHeaders.XAmzContentSha256, SigningConstants.StreamingPayload);

        byte[] seedSignature = _signatureBuilder.CreateSignature(request);
        return new ChunkedStream(_config, _chunkedSigBuilder, request, seedSignature, input);
    }

    private static ChecksumAlgorithm ConfigureTrailingChecksum(IRequest request, IRequestTrailers? requestTrailers)
    {
        if (requestTrailers == null || request is not IHasChecksumProperties { ChecksumAlgorithm: not ChecksumAlgorithm.Unknown, Checksum: null } checksum)
            return ChecksumAlgorithm.Unknown;

        string checksumHeader = AmzHeaders.XAmzChecksum + checksum.ChecksumAlgorithm.ToString().ToLowerInvariant();

        if (request.Headers.ContainsKey(checksumHeader))
            return ChecksumAlgorithm.Unknown;

        if (requestTrailers.Trailers.TryGetValue(checksumHeader, out string? value) && value.Length > 0)
            return ChecksumAlgorithm.Unknown;

        ChecksumHashAlgorithm.GetBase64Length(checksum.ChecksumAlgorithm);
        requestTrailers.SetTrailer(checksumHeader, string.Empty);
        request.SetHeader(AmzHeaders.XAmzSdkCheckumAlgorithm, checksum.ChecksumAlgorithm.GetDisplayName());
        return checksum.ChecksumAlgorithm;
    }

    private static string GetContentEncoding(IRequest request)
    {
        if (!request.Headers.TryGetValue(HttpHeaders.ContentEncoding, out string? contentEncoding))
            return "aws-chunked";

        if (string.IsNullOrWhiteSpace(contentEncoding))
            return "aws-chunked";

        return contentEncoding.Split(',').Any(x => x.Trim().Equals("aws-chunked", StringComparison.OrdinalIgnoreCase)) ? contentEncoding : contentEncoding + ", aws-chunked";
    }
}