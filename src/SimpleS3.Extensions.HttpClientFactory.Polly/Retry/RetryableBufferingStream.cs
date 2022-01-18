using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;

/// <summary>Stream that will buffer / record data as it's read, and be able to seek in it afterwards Used for retrying forward-only streams</summary>
internal class RetryableBufferingStream : Stream
{
    private readonly MemoryStream _bufferStream;
    private readonly Stream _underlyingStream;
    private bool _buffered;

    public RetryableBufferingStream(Stream underlyingStream)
    {
        // Note: Maybe validation is superfluous?
        Validator.RequireThat(!underlyingStream.CanSeek, nameof(underlyingStream), $"The {nameof(RetryableBufferingStream)} should not be used on seekable streams");

        _underlyingStream = underlyingStream;
        _bufferStream = new MemoryStream();
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _bufferStream.Length;

    public override long Position
    {
        get => _bufferStream.Position;
        set => _bufferStream.Position = value;
    }

    private async Task ReadSourceAsync()
    {
        await _underlyingStream.CopyToAsync(_bufferStream).ConfigureAwait(false);
        _bufferStream.Seek(0, SeekOrigin.Begin);

        _buffered = true;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!_buffered)
            ReadSourceAsync().GetAwaiter().GetResult();

        return _bufferStream.Read(buffer, offset, count);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (!_buffered)
            await ReadSourceAsync().ConfigureAwait(false);

        return await _bufferStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _bufferStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}