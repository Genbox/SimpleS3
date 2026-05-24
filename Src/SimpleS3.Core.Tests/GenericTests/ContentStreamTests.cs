using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ContentStreamTests
{
    [Fact]
    public async Task ReadAsyncPassesCancellationTokenToInnerStream()
    {
        CapturingStream inner = new CapturingStream();
        await using ContentStream content = new ContentStream(inner, null);
        using CancellationTokenSource cts = new CancellationTokenSource();
        byte[] buffer = new byte[1];

        await content.ReadAsync(buffer, cts.Token).ConfigureAwait(false);

        Assert.Equal(cts.Token, inner.ReadToken);
    }

    [Fact]
    public async Task CopyToAsyncPassesCancellationTokenToInnerStream()
    {
        CapturingStream inner = new CapturingStream();
        await using ContentStream content = new ContentStream(inner, null);
        using MemoryStream destination = new MemoryStream();
        using CancellationTokenSource cts = new CancellationTokenSource();

        await content.CopyToAsync(destination, 81920, cts.Token).ConfigureAwait(false);

        Assert.Equal(cts.Token, inner.CopyToken);
    }

    [SuppressMessage("Performance", "CA1844:Provide memory-based overrides of async methods when subclassing \'Stream\'")]
    private sealed class CapturingStream : Stream
    {
        private readonly MemoryStream _inner = new MemoryStream([1]);

        public CancellationToken ReadToken { get; private set; }
        public CancellationToken CopyToken { get; private set; }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ReadToken = cancellationToken;
            return _inner.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            CopyToken = cancellationToken;
            return Task.CompletedTask;
        }

        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
        public override void Flush() => _inner.Flush();
    }
}