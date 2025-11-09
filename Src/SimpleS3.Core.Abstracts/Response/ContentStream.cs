namespace Genbox.SimpleS3.Core.Abstracts.Response;

public class ContentStream(Stream stream, long? length) : Stream
{
    public new static ContentStream Null { get; } = new ContentStream(Stream.Null, null);

    public override bool CanRead => stream.CanRead;
    public override bool CanSeek => stream.CanSeek;
    public override bool CanWrite => stream.CanWrite;

    public bool HasLength => length != null;

    /// <summary>
    /// Returns the length of the response if provided by the server. If not provided it will throw an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public override long Length => length ?? throw new InvalidOperationException("Length was not provided by the server");

    public override long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);
    public override void SetLength(long value) => throw new AbandonedMutexException("This is a read-only stream");
    public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);
    public override void Flush() => stream.Flush();
}