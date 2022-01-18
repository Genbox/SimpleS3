using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal class ChunkedStream : Stream
{
    private const string _newlineStr = "\r\n";
    private static readonly byte[] _newline = { 13, 10 };

    private const string _chunkSignature = ";chunk-signature=";
    private readonly byte[] _buffer;
    private readonly IChunkedSignatureBuilder _chunkedSigBuilder;
    private readonly int _chunkSize;

    private readonly int _headerSize;
    private readonly Stream _originalStream;
    private readonly IRequest _request;
    private readonly byte[] _seedSignature;
    private int _bufferLength = -1;
    private int _bufferPosition = -1;
    private bool _inputStreamConsumed;

    private bool _outputBufferIsTerminatingChunk;
    private long _position;
    private byte[] _previousSignature;

    public ChunkedStream(IOptions<SimpleS3Config> options, IChunkedSignatureBuilder chunkedSigBuilder, IRequest request, byte[] seedSignature, Stream originalStream)
    {
        Validator.RequireNotNull(options, nameof(options));
        Validator.RequireNotNull(chunkedSigBuilder, nameof(chunkedSigBuilder));
        Validator.RequireNotNull(request, nameof(request));
        Validator.RequireNotNull(seedSignature, nameof(seedSignature));
        Validator.RequireNotNull(originalStream, nameof(originalStream));

        _originalStream = originalStream;
        _chunkedSigBuilder = chunkedSigBuilder;
        _request = request;
        _seedSignature = seedSignature;
        _previousSignature = seedSignature;

        _chunkSize = options.Value.StreamingChunkSize;

        _headerSize = CalculateChunkHeaderLength(_chunkSize, seedSignature.Length * 2);
        _buffer = new byte[_chunkSize + _headerSize + _newline.Length];
    }

    public override bool CanRead => true;
    public override bool CanSeek => _originalStream.CanSeek;
    public override bool CanWrite => false;
    public override long Length => ComputeChunkedContentLength(_originalStream.Length);

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
        if (_bufferPosition == -1)
        {
            if (_inputStreamConsumed && _outputBufferIsTerminatingChunk)
                return 0;

            int totalRead = 0;
            if (!_inputStreamConsumed)
            {
                while (_headerSize + totalRead < _chunkSize && !_inputStreamConsumed)
                {
                    int remaining = Math.Min(_chunkSize, _buffer.Length - totalRead - _headerSize);
                    int read = await _originalStream.ReadAsync(_buffer, _headerSize + totalRead, remaining, cancellationToken).ConfigureAwait(false);

                    if (read == 0)
                        _inputStreamConsumed = true;
                    else
                        totalRead += read;
                }
            }

            // Calculate header, and place in buffers beginning
            _previousSignature = _chunkedSigBuilder.CreateChunkSignature(_request, _previousSignature, _buffer, _headerSize, totalRead);
            int headerSize = CreateChunkHeader(_previousSignature, totalRead, _buffer);

            // Append final newline
            Buffer.BlockCopy(_newline, 0, _buffer, _headerSize + totalRead, _newline.Length);

            // Move data payload in buffer, if header is smaller than expected
            if (_headerSize != headerSize)
                Buffer.BlockCopy(_buffer, _headerSize, _buffer, headerSize, totalRead + _newline.Length);

            _bufferLength = headerSize + totalRead + _newline.Length;
            _bufferPosition = 0;

            // Fill buffer from N (header size) position to its full length
            _outputBufferIsTerminatingChunk = _inputStreamConsumed && totalRead == 0;
        }

        //Use the smaller of either data we have remaining, or the count being asked for
        count = Math.Min(_bufferLength - _bufferPosition, count);

        //Copy part of our output buffer into buffer
        Buffer.BlockCopy(_buffer, _bufferPosition, buffer, offset, count);
        _bufferPosition += count;

        if (_bufferPosition >= _bufferLength)
            _bufferPosition = -1;

        return count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (!CanSeek)
            throw new NotSupportedException("ChunkedStream cannot seek, as the base stream cannot seek");

        if (offset != 0 && origin != SeekOrigin.Begin)
            throw new NotSupportedException("ChunkedStream can only seek to position 0");

        // Reset position
        _originalStream.Seek(0, SeekOrigin.Begin);
        _position = 0;

        // Reset EOF markers
        _outputBufferIsTerminatingChunk = false;
        _inputStreamConsumed = false;

        // Clear buffers
        _bufferPosition = -1;
        _bufferLength = -1;

        // Reset signature
        _previousSignature = _seedSignature;

        return 0;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    private static int CalculateChunkHeaderLength(long chunkSize, int signatureLength)
    {
        return chunkSize.ToString("X", NumberFormatInfo.InvariantInfo).Length
               + _chunkSignature.Length
               + signatureLength
               + _newline.Length;
    }

    /// <summary>
    /// Computes the total size of the data payload, including the chunk metadata. Called externally so as to be able to set the correct
    /// Content-Length header value.
    /// </summary>
    private long ComputeChunkedContentLength(long originalLength)
    {
        if (originalLength < 0)
            throw new ArgumentOutOfRangeException(nameof(originalLength), "Expected 0 or greater value for originalLength.");

        if (originalLength == 0)
            return CalculateChunkHeaderLength(0, 64) + _newline.Length;

        long maxSizeChunks = Math.DivRem(originalLength, _chunkSize, out long remainingBytes);
        int chunkSize = CalculateChunkHeaderLength(_chunkSize, 64) + _chunkSize + _newline.Length;
        long lastChunkSize = CalculateChunkHeaderLength(remainingBytes, 64) + remainingBytes + _newline.Length;
        int headerSize = CalculateChunkHeaderLength(0, 64) + _newline.Length;

        return maxSizeChunks * chunkSize + (remainingBytes > 0 ? lastChunkSize : 0) + headerSize;
    }

    private static int CreateChunkHeader(byte[] chunkSignature, int contentLength, byte[] buffer)
    {
        StringBuilder chunkHeader = StringBuilderPool.Shared.Rent(100);

        chunkHeader
            .Append(contentLength.ToString("X", CultureInfo.InvariantCulture))
            .Append(_chunkSignature)
            .Append(chunkSignature.HexEncode())
            .Append(_newlineStr);

        string value = StringBuilderPool.Shared.ReturnString(chunkHeader);

        return Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);
    }
}