using System.Security.Cryptography;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class UploadPartCopyTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task UploadPartCopyRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] content = RandomNumberGenerator.GetBytes(4096);
        await CopyAndVerifyAsync(client, bucket, "source", content, content, [null]);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task CopySourceRange(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const int partSize = 5 * 1024 * 1024;
        const int offset = 257;
        const int contentLength = partSize + 1024;
        byte[] sourceBytes = RandomNumberGenerator.GetBytes(contentLength + (2 * offset));
        byte[] expected = sourceBytes.AsSpan(offset, contentLength).ToArray();

        // The first part meets the 5 MiB minimum; the final part may be smaller.
        string[] ranges = [$"bytes={offset}-{(offset + partSize) - 1}", $"bytes={offset + partSize}-{(offset + contentLength) - 1}"];
        await CopyAndVerifyAsync(client, bucket, "source", sourceBytes, expected, ranges);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task SourceKeyWithSpecialCharacters(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] content = RandomNumberGenerator.GetBytes(4096);
        await CopyAndVerifyAsync(client, bucket, "copy source/plus+percent% unicode-æ", content, content, [null]);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi, 0, 10_001)]
    public async Task InvalidPartNumberRejectedByClient(S3Provider _, string bucket, ISimpleClient client, int partNumber)
    {
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() => client.UploadPartCopyAsync(bucket, "source", bucket, "destination", partNumber, "unused-upload-id", token: TestContext.Current.CancellationToken));
        Assert.Equal("partNumber", exception.ParamName);
    }

    private static async Task CopyAndVerifyAsync(ISimpleClient client, string bucket, string sourceKey, byte[] sourceBytes, byte[] expected, string?[] ranges)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        const string destinationKey = "destination";
        await using MemoryStream source = new MemoryStream(sourceBytes);
        PutObjectResponse put = await client.PutObjectAsync(bucket, sourceKey, source, token: token);
        Assert.True(put.IsSuccess, put.Error?.Message);

        CreateMultipartUploadResponse initiated = await client.CreateMultipartUploadAsync(bucket, destinationKey, token: token);
        Assert.True(initiated.IsSuccess, initiated.Error?.Message);
        string uploadId = Assert.IsType<string>(initiated.UploadId);
        List<S3PartInfo> parts = [];

        for (int partNumber = 1; partNumber <= ranges.Length; partNumber++)
        {
            UploadPartCopyResponse copied = await client.UploadPartCopyAsync(bucket, sourceKey, bucket, destinationKey, partNumber, uploadId, request => request.CopySourceRange = ranges[partNumber - 1], token);
            Assert.True(copied.IsSuccess, copied.Error?.Message);
            Assert.False(string.IsNullOrEmpty(copied.ETag));
            parts.Add(new S3PartInfo(copied.ETag, partNumber));
        }

        CompleteMultipartUploadResponse complete = await client.CompleteMultipartUploadAsync(bucket, destinationKey, uploadId, parts, token: token);
        Assert.True(complete.IsSuccess, complete.Error?.Message);

        await AssertObjectContentAsync(client, bucket, destinationKey, expected);
    }
}