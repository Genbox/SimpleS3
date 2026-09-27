using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Errors;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class MultipartReadTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartDownloadRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] data = await UploadAsync(client, bucket);
        await using MemoryStream output = new MemoryStream();

        await foreach (GetObjectResponse response in client.MultipartDownloadAsync(bucket, "source", output, token: TestContext.Current.CancellationToken))
        {
            using (response)
                Assert.True(response.IsSuccess, response.Error?.Message);
        }

        Assert.Equal(data, output.ToArray());
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetPart(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] data = await UploadAsync(client, bucket);
        using GetObjectResponse response = await client.GetObjectAsync(bucket, "source", request => request.PartNumber = 2, TestContext.Current.CancellationToken);
        Assert.Equal(206, response.StatusCode);
        Assert.Equal(data.AsSpan(5 * 1024 * 1024).ToArray(), await response.Content.AsDataAsync(TestContext.Current.CancellationToken));
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task HeadReportsPartCount(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        await UploadAsync(client, bucket);
        HeadObjectResponse response = await client.HeadObjectAsync(bucket, "source", request => request.PartNumber = 1, TestContext.Current.CancellationToken);
        Assert.Equal(206, response.StatusCode);
        Assert.Equal(2, response.NumberOfParts);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetRejectsZeroPartNumber(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        await UploadAsync(client, bucket);
        using GetObjectResponse response = await client.GetObjectAsync(bucket, "source", request => request.PartNumber = 0, TestContext.Current.CancellationToken);
        Assert.Equal(400, response.StatusCode);
        Assert.IsType<InvalidArgumentError>(response.Error);
    });

    private static async Task<byte[]> UploadAsync(ISimpleClient client, string bucket)
    {
        byte[] data = new byte[(5 * 1024 * 1024) + 1024];
        data[0] = 1;
        data[^1] = 2;
        await using MemoryStream source = new MemoryStream(data);
        CompleteMultipartUploadResponse response = await client.MultipartUploadAsync(bucket, "source", source, 5 * 1024 * 1024, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        return data;
    }
}