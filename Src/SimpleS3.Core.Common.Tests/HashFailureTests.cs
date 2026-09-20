using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class HashFailureTests
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public void NonSeekableHashingConsumesOnlyWhenRestorationIsNotRequested(bool sha256, bool restore, bool readablePosition)
    {
        using NonSeekableStream stream = new NonSeekableStream(readablePosition);

        if (restore)
        {
            Assert.Throws<NotSupportedException>(() => Hash(stream, sha256, true));
            Assert.Equal(0, stream.BytesRead);
        }
        else
        {
            string digest = Convert.ToHexString(Hash(stream, sha256, false));
            Assert.Equal(sha256 ? "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD" : "900150983CD24FB0D6963F7D28E17F72", digest);
            Assert.Equal(3, stream.BytesRead);
        }

        Assert.True(stream.CanRead);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void FailedReadPreservesExceptionAndDoesNotContaminateNextHash(bool sha256, bool restore)
    {
        using FailingStream stream = new FailingStream();
        stream.Position = 2;
        IOException error = Assert.Throws<IOException>(() => Hash(stream, sha256, restore));
        Assert.Same(stream.Failure, error);
        Assert.Equal(restore ? 2 : 3, stream.Position);
        Assert.True(stream.CanRead);
        using MemoryStream next = new MemoryStream("abc"u8.ToArray());
        string digest = Convert.ToHexString(Hash(next, sha256, true));
        Assert.Equal(sha256 ? "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD" : "900150983CD24FB0D6963F7D28E17F72", digest);
        Assert.Equal(0, next.Position);
    }

    private static byte[] Hash(Stream stream, bool sha256, bool restore) => sha256 ? CryptoHelper.Sha256Hash(stream, restore) : CryptoHelper.Md5Hash(stream, restore);

    private sealed class NonSeekableStream(bool readablePosition) : MemoryStream("abc"u8.ToArray())
    {
        public int BytesRead { get; private set; }
        public override bool CanSeek => false;

        public override long Position
        {
            get => readablePosition ? base.Position : throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = base.Read(buffer, offset, count);
            BytesRead += read;
            return read;
        }

        public override int Read(Span<byte> buffer)
        {
            int read = base.Read(buffer);
            BytesRead += read;
            return read;
        }
    }

    private sealed class FailingStream() : MemoryStream("XXabc"u8.ToArray())
    {
        private bool _read;
        public IOException Failure { get; } = new IOException("Synthetic read failure");

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_read) throw Failure;
            _read = true;
            return base.Read(buffer, offset, Math.Min(count, 1));
        }

        public override int Read(Span<byte> buffer)
        {
            if (_read) throw Failure;
            _read = true;
            return base.Read(buffer[..Math.Min(buffer.Length, 1)]);
        }
    }
}