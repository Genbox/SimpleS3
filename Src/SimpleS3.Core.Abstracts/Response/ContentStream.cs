using System.Diagnostics.CodeAnalysis;

namespace Genbox.SimpleS3.Core.Abstracts.Response;

/// <summary>Provides the <c>ContentStream</c> API.</summary>
public class ContentStream(Stream stream, long? length, IDisposable? owner = null) : Stream
{
    private bool _disposed;

    /// <summary>Provides the <c>Null</c> API.</summary>
    public new static ContentStream Null { get; } = new ContentStream(Stream.Null, null);

    /// <summary>Provides the <c>CanRead</c> API.</summary>
    public override bool CanRead => stream.CanRead;
    /// <summary>Provides the <c>CanSeek</c> API.</summary>
    public override bool CanSeek => stream.CanSeek;
    /// <summary>Provides the <c>CanWrite</c> API.</summary>
    public override bool CanWrite => stream.CanWrite;

    /// <summary>Provides the <c>HasLength</c> API.</summary>
    public bool HasLength => length != null;

    /// <summary>
    /// Returns the length of the response if provided by the server. If not provided it will throw an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public override long Length => length ?? throw new InvalidOperationException("Length was not provided by the server");

    /// <summary>Provides the <c>Position</c> API.</summary>
    public override long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    /// <summary>Provides the <c>Read</c> API.</summary>
    public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);
    /// <summary>Provides the <c>ReadAsync</c> API.</summary>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => stream.ReadAsync(buffer, offset, count, cancellationToken);
    /// <summary>Provides the <c>Seek</c> API.</summary>
    public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);
    /// <summary>Provides the <c>SetLength</c> API.</summary>
    public override void SetLength(long value) => throw new AbandonedMutexException("This is a read-only stream");
    /// <summary>Provides the <c>Write</c> API.</summary>
    public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);
    /// <summary>Provides the <c>WriteAsync</c> API.</summary>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => stream.WriteAsync(buffer, offset, count, cancellationToken);
    /// <summary>Provides the <c>Flush</c> API.</summary>
    public override void Flush() => stream.Flush();
    /// <summary>Provides the <c>FlushAsync</c> API.</summary>
    public override Task FlushAsync(CancellationToken cancellationToken) => stream.FlushAsync(cancellationToken);
    /// <summary>Provides the <c>CopyToAsync</c> API.</summary>
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => stream.CopyToAsync(destination, bufferSize, cancellationToken);

    /// <summary>Provides the <c>Dispose</c> API.</summary>
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007:Don\'t dispose injected", Justification = "ContentStream is a wrapper. It has to manage disposal.")]
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;

        if (disposing)
        {
            stream.Dispose();
            owner?.Dispose();
        }

        base.Dispose(disposing);
    }
}