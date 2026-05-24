using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Tests;

public class RetryableBufferingStreamTests
{
    [Fact]
    public async Task ReadAsyncPassesCancellationTokenWhileBuffering()
    {
        CapturingCancellationStream source = new CapturingCancellationStream();
        await using RetryableBufferingStream stream = new RetryableBufferingStream(source, 1024);
        using CancellationTokenSource cts = new CancellationTokenSource();
        byte[] buffer = new byte[1];

        await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token);

        Assert.Equal(cts.Token, source.CapturedToken);
    }

    [Fact]
    public void SpilledTempFileIsRemovedOnDispose()
    {
        RetryableBufferingStream stream = new RetryableBufferingStream(new NonSeekableDataStream("hello"u8.ToArray()), 1);
        byte[] buffer = new byte[5];

        stream.Read(buffer, 0, buffer.Length);
        string path = Assert.IsType<string>(stream.TempFilePath);

        stream.Dispose();

        Assert.False(File.Exists(path));
    }

    private sealed class CapturingCancellationStream : Stream
    {
        public CancellationToken CapturedToken { get; private set; }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            CapturedToken = cancellationToken;
            return Task.FromResult(0);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            CapturedToken = cancellationToken;
            return ValueTask.FromResult(0);
        }

        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }

    private sealed class NonSeekableDataStream(byte[] data) : Stream
    {
        private readonly MemoryStream _inner = new MemoryStream(data);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }
}