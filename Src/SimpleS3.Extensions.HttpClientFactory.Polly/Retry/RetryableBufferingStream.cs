using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;

/// <summary>Stream that will buffer / record data as it's read, and be able to seek in it afterwards Used for retrying forward-only streams</summary>
internal sealed class RetryableBufferingStream : Stream
{
    private readonly MemoryStream _bufferStream;
    private readonly Stream _underlyingStream;
    private bool _buffered;
    private bool _disposed;

    public RetryableBufferingStream(Stream underlyingStream)
    {
        Validator.RequireThat(!underlyingStream.CanSeek, $"The {nameof(RetryableBufferingStream)} should not be used on seekable streams");

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

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;

        if (disposing)
            _bufferStream.Dispose();

        base.Dispose(disposing);
    }

    private async Task ReadSourceAsync()
    {
        await _underlyingStream.CopyToAsync(_bufferStream).ConfigureAwait(false);
        _bufferStream.Seek(0, SeekOrigin.Begin);
        _buffered = true;
    }

    private void ReadSource()
    {
        _underlyingStream.CopyTo(_bufferStream);
        _bufferStream.Seek(0, SeekOrigin.Begin);
        _buffered = true;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!_buffered)
            ReadSource();

        return _bufferStream.Read(buffer, offset, count);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (!_buffered)
            await ReadSourceAsync().ConfigureAwait(false);

        return await _bufferStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

    public override long Seek(long offset, SeekOrigin origin) => _bufferStream.Seek(offset, origin);

    public override void SetLength(long value)
    {
        throw new NotSupportedException("SetLength is not supported");
    }

    public override void Flush()
    {
        throw new NotSupportedException("Flush is not supported");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Write is not supported");
    }
}