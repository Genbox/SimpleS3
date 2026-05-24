using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ChunkedStreamTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(8192)]
    [InlineData(8193)]
    public void EmittedByteCountMatchesLength(int payloadLength)
    {
        byte[] payload = Enumerable.Repeat((byte)'a', payloadLength).ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        using ChunkedStream stream = new ChunkedStream(config, new StableChunkedSignatureBuilder(), new PutObjectRequest("bucket", "object", null), new byte[32], new MemoryStream(payload));
        using MemoryStream output = new MemoryStream();

        stream.CopyTo(output);

        Assert.Equal(stream.Length, output.Length);
    }

    [Fact]
    public void ChunkedWrapperRejectsStreamsWithoutLength()
    {
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());

        NotSupportedException ex = Assert.Throws<NotSupportedException>(() => wrapper.Wrap(new UnknownLengthStream(), new PutObjectRequest("bucket", "object", null)));
        Assert.Contains("exposes Length", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class StableChunkedSignatureBuilder : IChunkedSignatureBuilder
    {
        public byte[] CreateChunkSignature(IRequest request, byte[] previousSignature, byte[] content, int offset, int length) => new byte[32];
    }

    private sealed class StableSignatureBuilder : ISignatureBuilder
    {
        public byte[] CreateSignature(IRequest request, bool enablePayloadSignature = true) => new byte[32];
    }

    private sealed class UnknownLengthStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }
}