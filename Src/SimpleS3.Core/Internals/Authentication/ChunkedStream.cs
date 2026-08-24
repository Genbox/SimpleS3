using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed class ChunkedStream : Stream
{
    private const string _newlineStr = "\r\n";
    internal const int SignatureLength = 64;

    private const string _chunkSignature = ";chunk-signature=";
    private static readonly byte[] _newline = [13, 10];
    private readonly byte[] _buffer;
    private readonly IChunkedSignatureBuilder _chunkedSigBuilder;
    private readonly HashAlgorithm? _checksum;
    private readonly int _chunkSize;
    private readonly ChecksumAlgorithm _checksumAlgorithm;
    private readonly string? _checksumTrailer;
    private readonly IRequestTrailers? _requestTrailers;
    private readonly ITrailerSignatureBuilder? _trailerSigBuilder;
    private readonly IReadOnlyDictionary<string, string> _trailers;

    private readonly int _headerSize;
    private readonly long _decodedLength;
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "The request stream is caller-owned and can be reused after ChunkedStream is disposed.")]
    private readonly Stream _originalStream;
    private readonly long _originalStreamPosition;
    private readonly IRequest _request;
    private readonly byte[] _seedSignature;
    private int _bufferLength = -1;
    private int _bufferPosition = -1;
    private bool _inputStreamConsumed;

    private bool _outputBufferIsTerminatingChunk;
    private long _position;
    private byte[] _previousSignature;
    private byte[]? _trailerBuffer;
    private int _trailerBufferPosition;
    private bool _trailingHeadersConsumed;

    public ChunkedStream(SimpleS3Config config, IChunkedSignatureBuilder chunkedSigBuilder, IRequest request, byte[] seedSignature, Stream originalStream)
    {
        Validator.RequireNotNull(config);
        Validator.RequireNotNull(chunkedSigBuilder);
        Validator.RequireNotNull(request);
        Validator.RequireNotNull(seedSignature);
        Validator.RequireNotNull(originalStream);

        _originalStream = originalStream;
        _originalStreamPosition = originalStream.CanSeek ? originalStream.Position : 0;
        _decodedLength = originalStream.Length - _originalStreamPosition;
        _chunkedSigBuilder = chunkedSigBuilder;
        _trailerSigBuilder = chunkedSigBuilder as ITrailerSignatureBuilder;
        _request = request;
        _requestTrailers = request as IRequestTrailers;
        _trailers = _requestTrailers?.Trailers ?? EmptyTrailers;
        (_checksumAlgorithm, _checksumTrailer, _checksum) = CreateChecksum(request, _trailers);
        _seedSignature = seedSignature;
        _previousSignature = seedSignature;
        _trailingHeadersConsumed = _trailers.Count == 0;

        _chunkSize = config.StreamingChunkSize;

        _headerSize = CalculateChunkHeaderLength(_chunkSize, seedSignature.Length * 2);
        _buffer = new byte[_chunkSize + _headerSize + _newline.Length];
    }

    public override bool CanRead => true;
    public override bool CanSeek => _originalStream.CanSeek;
    public override bool CanWrite => false;
    public override long Length => ComputeChunkedContentLength(_decodedLength, _chunkSize, SignatureLength, _trailers, _checksumAlgorithm);

    public override long Position
    {
        get => _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override void Flush()
    {
        // Do nothing
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (count == 0)
            return 0;

        if (_bufferPosition == -1)
        {
            if (_inputStreamConsumed && _outputBufferIsTerminatingChunk)
            {
                if (!_trailingHeadersConsumed)
                    return ReadTrailingHeaders(buffer, offset, count);

                return 0;
            }

            int totalRead = 0;
            if (!_inputStreamConsumed)
            {
                while (totalRead < _chunkSize && !_inputStreamConsumed)
                {
                    int remaining = _chunkSize - totalRead;
                    int read = await _originalStream.ReadAsync(_buffer, _headerSize + totalRead, remaining, cancellationToken).ConfigureAwait(false);

                    if (read == 0)
                        _inputStreamConsumed = true;
                    else
                        totalRead += read;
                }
            }

            UpdateChecksum(totalRead);

            // Calculate header, and place in buffers beginning
            _previousSignature = _chunkedSigBuilder.CreateChunkSignature(_request, _previousSignature, _buffer, _headerSize, totalRead);
            int headerSize = CreateChunkHeader(_previousSignature, totalRead, _buffer);

            bool appendFinalNewline = !_inputStreamConsumed || totalRead != 0 || _trailers.Count == 0;
            int finalNewlineLength = appendFinalNewline ? _newline.Length : 0;

            if (appendFinalNewline)
                Buffer.BlockCopy(_newline, 0, _buffer, _headerSize + totalRead, _newline.Length);

            // Move data payload in buffer, if header is smaller than expected
            if (_headerSize != headerSize)
                Buffer.BlockCopy(_buffer, _headerSize, _buffer, headerSize, totalRead + finalNewlineLength);

            _bufferLength = headerSize + totalRead + finalNewlineLength;
            _bufferPosition = 0;

            // Fill buffer from N (header size) position to its full length
            _outputBufferIsTerminatingChunk = _inputStreamConsumed && totalRead == 0;
        }

        //Use the smaller of either data we have remaining, or the count being asked for
        count = Math.Min(_bufferLength - _bufferPosition, count);

        //Copy part of our output buffer into buffer
        Buffer.BlockCopy(_buffer, _bufferPosition, buffer, offset, count);
        _bufferPosition += count;
        _position += count;

        if (_bufferPosition >= _bufferLength)
            _bufferPosition = -1;

        return count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count == 0)
            return 0;

        if (_bufferPosition == -1)
        {
            if (_inputStreamConsumed && _outputBufferIsTerminatingChunk)
            {
                if (!_trailingHeadersConsumed)
                    return ReadTrailingHeaders(buffer, offset, count);

                return 0;
            }

            int totalRead = 0;
            if (!_inputStreamConsumed)
            {
                while (totalRead < _chunkSize && !_inputStreamConsumed)
                {
                    int remaining = _chunkSize - totalRead;
                    int read = _originalStream.Read(_buffer, _headerSize + totalRead, remaining);

                    if (read == 0)
                        _inputStreamConsumed = true;
                    else
                        totalRead += read;
                }
            }

            UpdateChecksum(totalRead);

            // Calculate header, and place in buffers beginning
            _previousSignature = _chunkedSigBuilder.CreateChunkSignature(_request, _previousSignature, _buffer, _headerSize, totalRead);
            int headerSize = CreateChunkHeader(_previousSignature, totalRead, _buffer);

            bool appendFinalNewline = !_inputStreamConsumed || totalRead != 0 || _trailers.Count == 0;
            int finalNewlineLength = appendFinalNewline ? _newline.Length : 0;

            if (appendFinalNewline)
                Buffer.BlockCopy(_newline, 0, _buffer, _headerSize + totalRead, _newline.Length);

            // Move data payload in buffer, if header is smaller than expected
            if (_headerSize != headerSize)
                Buffer.BlockCopy(_buffer, _headerSize, _buffer, headerSize, totalRead + finalNewlineLength);

            _bufferLength = headerSize + totalRead + finalNewlineLength;
            _bufferPosition = 0;

            // Fill buffer from N (header size) position to its full length
            _outputBufferIsTerminatingChunk = _inputStreamConsumed && totalRead == 0;
        }

        //Use the smaller of either data we have remaining, or the count being asked for
        count = Math.Min(_bufferLength - _bufferPosition, count);

        //Copy part of our output buffer into buffer
        Buffer.BlockCopy(_buffer, _bufferPosition, buffer, offset, count);
        _bufferPosition += count;
        _position += count;

        if (_bufferPosition >= _bufferLength)
            _bufferPosition = -1;

        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (!CanSeek)
            throw new NotSupportedException("ChunkedStream cannot seek, as the base stream cannot seek");

        if (offset != 0 || origin != SeekOrigin.Begin)
            throw new NotSupportedException("ChunkedStream can only seek to position 0");

        // Reset position
        _originalStream.Seek(_originalStreamPosition, SeekOrigin.Begin);
        _position = 0;

        // Reset EOF markers
        _outputBufferIsTerminatingChunk = false;
        _inputStreamConsumed = false;

        // Clear buffers
        _bufferPosition = -1;
        _bufferLength = -1;
        _trailerBuffer = null;
        _trailerBufferPosition = 0;
        _trailingHeadersConsumed = _trailers.Count == 0;
        _checksum?.Initialize();

        // Reset signature
        _previousSignature = _seedSignature;

        return 0;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException(nameof(ChunkedStream) + " does not support length");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException(nameof(ChunkedStream) + " does not support writing");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _checksum?.Dispose();

        base.Dispose(disposing);
    }

    private static int CalculateChunkHeaderLength(long chunkSize, int signatureLength) =>
        chunkSize.ToString("X", NumberFormatInfo.InvariantInfo).Length
        + _chunkSignature.Length
        + signatureLength
        + _newline.Length;

    /// <summary>Computes the total size of the data payload, including the chunk metadata. Called externally so as to be able to set the correct Content-Length header value.</summary>
    internal static long ComputeChunkedContentLength(long originalLength, int chunkSize, int signatureLength, IReadOnlyDictionary<string, string>? trailers = null, ChecksumAlgorithm checksumAlgorithm = ChecksumAlgorithm.Unknown)
    {
        if (originalLength < 0)
            throw new ArgumentOutOfRangeException(nameof(originalLength), "Expected 0 or greater value for originalLength.");

        bool hasTrailers = trailers?.Count > 0;
        long terminatingChunkSize = CalculateChunkHeaderLength(0, signatureLength) + (hasTrailers ? 0 : _newline.Length);
        long chunkedContentLength;

        if (originalLength == 0)
            chunkedContentLength = terminatingChunkSize;
        else
        {
            long maxSizeChunks = Math.DivRem(originalLength, chunkSize, out long remainingBytes);
            int fullChunkSize = CalculateChunkHeaderLength(chunkSize, signatureLength) + chunkSize + _newline.Length;
            long lastChunkSize = CalculateChunkHeaderLength(remainingBytes, signatureLength) + remainingBytes + _newline.Length;

            chunkedContentLength = (maxSizeChunks * fullChunkSize) + (remainingBytes > 0 ? lastChunkSize : 0) + terminatingChunkSize;
        }

        if (!hasTrailers)
            return chunkedContentLength;

        long trailerLength = _newline.Length;

        string? checksumTrailer = checksumAlgorithm == ChecksumAlgorithm.Unknown ? null : GetChecksumTrailer(checksumAlgorithm);

        foreach (KeyValuePair<string, string> trailer in trailers!)
        {
            int valueLength = trailer.Key == checksumTrailer && trailer.Value.Length == 0
                ? ChecksumHashAlgorithm.GetBase64Length(checksumAlgorithm)
                : Encoding.UTF8.GetByteCount(trailer.Value);
            trailerLength += CalculateTrailerHeaderLength(trailer.Key, valueLength);
        }

        trailerLength += CalculateTrailerHeaderLength(SigningConstants.TrailerSignatureHeader, new string('0', signatureLength));

        return chunkedContentLength + trailerLength;
    }

    private static int CalculateTrailerHeaderLength(string key, string value) => CalculateTrailerHeaderLength(key, Encoding.UTF8.GetByteCount(value));

    private static int CalculateTrailerHeaderLength(string key, int valueLength) => Encoding.UTF8.GetByteCount(key) + 1 + valueLength + _newline.Length;

    private static int CreateChunkHeader(byte[] chunkSignature, int contentLength, byte[] buffer)
    {
        StringBuilder chunkHeader = StringBuilderPool.Shared.Rent(100);

        chunkHeader
            .Append(contentLength.ToString("X", NumberFormatInfo.InvariantInfo))
            .Append(_chunkSignature)
            .Append(chunkSignature.HexEncode())
            .Append(_newlineStr);

        string value = StringBuilderPool.Shared.ReturnString(chunkHeader, clearContents: true);

        return Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);
    }

    private int ReadTrailingHeaders(byte[] buffer, int offset, int count)
    {
        _trailerBuffer ??= CreateTrailerBuffer();

        int lengthRemaining = _trailerBuffer.Length - _trailerBufferPosition;
        if (lengthRemaining == 0)
        {
            _trailingHeadersConsumed = true;
            return 0;
        }

        count = Math.Min(lengthRemaining, count);
        Buffer.BlockCopy(_trailerBuffer, _trailerBufferPosition, buffer, offset, count);
        _trailerBufferPosition += count;
        _position += count;

        if (_trailerBufferPosition >= _trailerBuffer.Length)
            _trailingHeadersConsumed = true;

        return count;
    }

    private byte[] CreateTrailerBuffer()
    {
        byte[]? trailerSignature = null;

        try
        {
            CompleteChecksum();
            if (_trailerSigBuilder == null)
                throw new NotSupportedException($"{_chunkedSigBuilder.GetType().Name} does not support signing trailing headers.");

            trailerSignature = _trailerSigBuilder.CreateTrailerSignature(_request, _previousSignature);

            StringBuilder trailer = StringBuilderPool.Shared.Rent(200);

            foreach (KeyValuePair<string, string> item in _trailers.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                trailer.Append(item.Key)
                       .Append(SigningConstants.Colon)
                       .Append(item.Value)
                       .Append(_newlineStr);
            }

            trailer.Append(SigningConstants.TrailerSignatureHeader)
                   .Append(SigningConstants.Colon)
                   .Append(trailerSignature.HexEncode())
                   .Append(_newlineStr)
                   .Append(_newlineStr);

            string value = StringBuilderPool.Shared.ReturnString(trailer, clearContents: true);
            return Encoding.UTF8.GetBytes(value);
        }
        finally
        {
            if (_checksum != null && _checksumTrailer != null && _requestTrailers != null)
                _requestTrailers.SetTrailer(_checksumTrailer, string.Empty);

            if (trailerSignature != null)
                Array.Clear(trailerSignature, 0, trailerSignature.Length);
        }
    }

    private void UpdateChecksum(int length)
    {
        if (_checksum != null && length > 0)
            _checksum.TransformBlock(_buffer, _headerSize, length, _buffer, _headerSize);
    }

    private void CompleteChecksum()
    {
        if (_checksum == null || _checksumTrailer == null || _requestTrailers == null)
            return;

        _checksum.TransformFinalBlock([], 0, 0);
        _requestTrailers.SetTrailer(_checksumTrailer, Convert.ToBase64String(_checksum.Hash));
    }

    private static (ChecksumAlgorithm Algorithm, string? Trailer, HashAlgorithm? Hash) CreateChecksum(IRequest request, IReadOnlyDictionary<string, string> trailers)
    {
        if (request is not IHasChecksumProperties { ChecksumAlgorithm: not ChecksumAlgorithm.Unknown, Checksum: null } checksum)
            return (ChecksumAlgorithm.Unknown, null, null);

        string checksumTrailer = GetChecksumTrailer(checksum.ChecksumAlgorithm);

        if (!trailers.TryGetValue(checksumTrailer, out string? value) || value.Length != 0)
            return (ChecksumAlgorithm.Unknown, null, null);

        return (checksum.ChecksumAlgorithm, checksumTrailer, ChecksumHashAlgorithm.Create(checksum.ChecksumAlgorithm));
    }

    private static string GetChecksumTrailer(ChecksumAlgorithm algorithm) => AmzHeaders.XAmzChecksum + algorithm.ToString().ToLowerInvariant();

    private static IReadOnlyDictionary<string, string> EmptyTrailers { get; } = new Dictionary<string, string>(0);
}