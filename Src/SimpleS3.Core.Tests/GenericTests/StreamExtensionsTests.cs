using System.Text;
using Genbox.SimpleS3.Core.Extensions;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class StreamExtensionsTests
{
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
        byte[] body = encoding.GetBytes("text with Unicode: ‚ ??");
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
}