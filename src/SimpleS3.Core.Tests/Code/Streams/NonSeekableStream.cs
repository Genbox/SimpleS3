namespace Genbox.SimpleS3.Core.Tests.Code.Streams;

internal class NonSeekableStream : Stream
{
    private readonly MemoryStream _backingStream;

    public NonSeekableStream(byte[] data)
    {
        _backingStream = new MemoryStream(data);
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _backingStream.Length;

    public override long Position
    {
        get => _backingStream.Position;
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _backingStream.Read(buffer, offset, count);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _backingStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}