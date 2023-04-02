using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.Tests.Code.Streams;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

public class NonSeekableStreamTests : OfflineTestBase
{
    public NonSeekableStreamTests(ITestOutputHelper outputHelper) : base(outputHelper) {}

    protected override void ConfigureConfig(SimpleS3Config config)
    {
        //We force streaming signatures as it is the only one that supports non-seekable streams
        config.StreamingChunkSize = 8096;
        config.PayloadSignatureMode = SignatureMode.StreamingSignature;

        base.ConfigureConfig(config);
    }

    [Fact]
    public async Task SendNonSeekableStream()
    {
        byte[] data = new byte[1024 * 10];
        Array.Fill(data, (byte)'A');

        //We test if it is possible send a non-seekable stream. This should succeed as we use ChunkedStream
        PutObjectResponse resp = await ObjectClient.PutObjectAsync(BucketName, nameof(SendNonSeekableStream), new NonSeekableStream(data)).ConfigureAwait(false);
        Assert.True(resp.IsSuccess);
    }
}