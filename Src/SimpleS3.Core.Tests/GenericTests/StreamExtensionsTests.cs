using System.Text;
using Genbox.SimpleS3.Core.Extensions;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class StreamExtensionsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(81921)]
    public async Task AsDataReadsRemainingBytesWithoutOwningInput(int remaining)
    {
        byte[] original = Enumerable.Range(0, remaining + 3).Select(x => (byte)(x % 251)).ToArray();
        await using MemoryStream input = new MemoryStream(original);
        input.Position = 3;
        byte[] result = await input.AsDataAsync(TestContext.Current.CancellationToken);
        Assert.Equal(original.AsSpan(3).ToArray(), result);
        Assert.Equal(input.Length, input.Position);
        Assert.True(input.CanRead);

        if (remaining > 0)
        {
            byte before = original[3];
            result[0] ^= 0xff;
            input.Position = 3;
            Assert.Equal(before, input.ReadByte());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ConversionHonorsCancellationAndLeavesInputOpen(bool text)
    {
        await using MemoryStream input = new MemoryStream("input"u8.ToArray());
        input.Position = 1;
        using CancellationTokenSource cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        if (text)
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => input.AsStringAsync(token: cancellation.Token));
        else
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => input.AsDataAsync(cancellation.Token));
        Assert.True(input.CanRead);
        Assert.Equal(1, input.Position);
    }

    [Theory]
    [InlineData("utf-8", false)]
    [InlineData("utf-8", true)]
    [InlineData("utf-16", true)]
    [InlineData("utf-32", true)]
    public async Task AsStringPreservesEncodingPreambleAndCurrentPosition(string encodingName, bool includePreamble)
    {
        Encoding encoding = Encoding.GetEncoding(encodingName);
        byte[] body = encoding.GetBytes("text with Unicode: é 💩");
        byte[] bytes = includePreamble ? encoding.GetPreamble().Concat(body).ToArray() : body;
        await using MemoryStream input = new MemoryStream(new byte[] { 1, 2, 3 }.Concat(bytes).ToArray());
        input.Position = 3;

        string actual = await input.AsStringAsync(encoding, TestContext.Current.CancellationToken);

        Assert.Equal(encoding.GetString(bytes), actual);
        Assert.Equal(input.Length, input.Position);
        Assert.True(input.CanRead);
    }

    [Fact]
    public async Task AsStringPreservesEmptyInputAndDecoderFallback()
    {
        await using MemoryStream empty = new MemoryStream();
        Assert.Equal(string.Empty, await empty.AsStringAsync(token: TestContext.Current.CancellationToken));
        await using MemoryStream invalid = new MemoryStream([0xff]);
        await Assert.ThrowsAsync<DecoderFallbackException>(() => invalid.AsStringAsync(new UTF8Encoding(false, true), TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task CopyToFileSupportsRelativePathsAndReplacesContent(bool childDirectory, bool existingFile)
    {
        string name = "SimpleS3-stream-test-" + Guid.NewGuid().ToString("N");
        string file = childDirectory ? Path.Combine(name, "download.bin") : name + ".bin";
        CancellationToken token = TestContext.Current.CancellationToken;
        byte[] expected = "download"u8.ToArray();

        try
        {
            if (existingFile)
            {
                if (childDirectory)
                    Directory.CreateDirectory(name);
                await File.WriteAllBytesAsync(file, "a longer existing download with trailing bytes"u8.ToArray(), token);
            }

            await using MemoryStream input = new MemoryStream(expected);
            await input.CopyToFileAsync(file, token);

            Assert.Equal(expected, await File.ReadAllBytesAsync(file, token));
        }
        finally
        {
            File.Delete(file);
            if (childDirectory && Directory.Exists(name))
                Directory.Delete(name);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PreCancelledFileCopyPreservesDestination(bool existing)
    {
        string directory = Path.Combine(Path.GetTempPath(), "simples3-cancel-" + Guid.NewGuid().ToString("N"));
        string file = Path.Combine(directory, "output.bin");
        byte[] original = "keep existing content"u8.ToArray();

        try
        {
            if (existing)
            {
                Directory.CreateDirectory(directory);
                await File.WriteAllBytesAsync(file, original, TestContext.Current.CancellationToken);
            }

            await using MemoryStream input = new MemoryStream("replacement"u8.ToArray());
            input.Position = 1;
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            await cancellation.CancelAsync();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => input.CopyToFileAsync(file, cancellation.Token));
            Assert.True(input.CanRead);
            Assert.Equal(1, input.Position);

            if (existing)
                Assert.Equal(original, await File.ReadAllBytesAsync(file, TestContext.Current.CancellationToken));
            else
            {
                Assert.False(File.Exists(file));
                Assert.False(Directory.Exists(directory));
            }
        }
        finally
        {
            if (File.Exists(file))
                File.Delete(file);
            if (Directory.Exists(directory))
                Directory.Delete(directory);
        }
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    public async Task HelpersHandleNonseekableShortReadsAndPreserveSourceOnFailure(int helper, bool fail)
    {
        byte[] data = Encoding.UTF8.GetBytes("aé😀z");
        await using ShortReadSource source = new ShortReadSource(data, fail);
        string file = Path.Combine(Path.GetTempPath(), "simples3-short-read-" + Guid.NewGuid().ToString("N"));

        try
        {
            if (helper == 2)
                await File.WriteAllBytesAsync(file, "long previous destination"u8.ToArray(), TestContext.Current.CancellationToken);

            async Task ConvertAsync()
            {
                if (helper == 0)
                    Assert.Equal(data, await source.AsDataAsync(TestContext.Current.CancellationToken));
                else if (helper == 1)
                    Assert.Equal("aé😀z", await source.AsStringAsync(token: TestContext.Current.CancellationToken));
                else
                    await source.CopyToFileAsync(file, TestContext.Current.CancellationToken);
            }

            if (fail)
                Assert.Same(source.Failure, await Assert.ThrowsAsync<IOException>(ConvertAsync));
            else
                await ConvertAsync();
            Assert.True(source.CanRead);
            Assert.False(source.CanSeek);
            Assert.Equal(fail ? 3 : data.Length, source.BytesRead);
            Assert.True(source.ReadCalls >= 2);
            if (helper == 2)
                Assert.Equal(fail ? data.AsSpan(0, 3).ToArray() : data, await File.ReadAllBytesAsync(file, TestContext.Current.CancellationToken));
        }
        finally
        {
            File.Delete(file);
        }
    }

    private sealed class ShortReadSource(byte[] data, bool fail) : Stream
    {
        private bool _disposed;
        public IOException Failure { get; } = new IOException("Synthetic read failure");
        public int BytesRead { get; private set; }
        public int ReadCalls { get; private set; }
        public override bool CanRead => !_disposed;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => ReadCore(buffer.AsSpan(offset, count));

        private int ReadCore(Span<byte> buffer)
        {
            ReadCalls++;
            if (fail && BytesRead >= 3)
                throw Failure;
            int size = Math.Min(3, Math.Min(buffer.Length, data.Length - BytesRead));
            data.AsSpan(BytesRead, size).CopyTo(buffer);
            BytesRead += size;
            return size;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(ReadCore(buffer.AsSpan(offset, count)));
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new ValueTask<int>(ReadCore(buffer.Span));
        }

        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            _disposed = true;
            base.Dispose(disposing);
        }
    }
}