using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;

/// <summary>Stream that will buffer / record data as it's read, and be able to seek in it afterwards Used for retrying forward-only streams</summary>
internal sealed class RetryableBufferingStream : Stream
{
    private readonly int _maxMemoryBufferSize;
    private readonly Stream _underlyingStream;
    private Stream _bufferStream;
    private bool _buffered;
    private bool _disposed;
    private string? _tempFilePath;

    internal string? TempFilePath => _tempFilePath;

    public RetryableBufferingStream(Stream underlyingStream, int maxMemoryBufferSize)
    {
        Validator.RequireThat(!underlyingStream.CanSeek, $"The {nameof(RetryableBufferingStream)} should not be used on seekable streams");
        Validator.RequireThat(maxMemoryBufferSize >= 0, $"The {nameof(maxMemoryBufferSize)} must be zero or greater");

        _underlyingStream = underlyingStream;
        _maxMemoryBufferSize = maxMemoryBufferSize;
        _bufferStream = new MemoryStream();
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;

    public override long Length
    {
        get
        {
            if (!_buffered)
                ReadSource();

            return _bufferStream.Length;
        }
    }

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
        {
            _bufferStream.Dispose();

            if (_tempFilePath != null)
                File.Delete(_tempFilePath);
        }

        base.Dispose(disposing);
    }

    private async Task ReadSourceAsync(CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[81920];
        int read;

        while ((read = await _underlyingStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            EnsureCapacity(read);
            await _bufferStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
        }

        _bufferStream.Seek(0, SeekOrigin.Begin);
        _buffered = true;
    }

    private void ReadSource()
    {
        byte[] buffer = new byte[81920];
        int read;

        while ((read = _underlyingStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            EnsureCapacity(read);
            _bufferStream.Write(buffer, 0, read);
        }

        _bufferStream.Seek(0, SeekOrigin.Begin);
        _buffered = true;
    }

    private void EnsureCapacity(int additionalBytes)
    {
        if (_bufferStream is not MemoryStream memoryStream || memoryStream.Length + additionalBytes <= _maxMemoryBufferSize)
            return;

        _tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        FileStream fileStream = new FileStream(_tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);

        try
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(fileStream);
            memoryStream.Dispose();
            _bufferStream = fileStream;
        }
        catch
        {
            fileStream.Dispose();
            throw;
        }
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
            await ReadSourceAsync(cancellationToken).ConfigureAwait(false);

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