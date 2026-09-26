using System.Diagnostics.CodeAnalysis;

namespace Genbox.ProviderTests.Code;

internal sealed class AuditingReadStream(Stream inner, string path) : Stream
{
    private readonly FileStream _audit = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
    private bool _disposed;

    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => inner.CanSeek;
    public override bool CanWrite => false;
    public override long Length => inner.Length;
    public override long Position { get => inner.Position; set => inner.Position = value; }

    public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

    public override int Read(Span<byte> buffer)
    {
        int read = inner.Read(buffer);
        _audit.Write(buffer[..read]);
        _audit.Flush();
        return read;
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int read = await inner.ReadAsync(buffer, cancellationToken);
        await _audit.WriteAsync(buffer[..read], cancellationToken);
        await _audit.FlushAsync(cancellationToken);
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);
    public override void Flush() => _audit.Flush();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007:Don't dispose injected", Justification = "The wrapper owns the underlying stream, preserving transport stream ownership.")]
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;

            try
            {
                inner.Dispose();
            }
            finally
            {
                _audit.Dispose();
            }
        }

        base.Dispose(disposing);
    }
}