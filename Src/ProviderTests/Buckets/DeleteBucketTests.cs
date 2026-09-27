using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class DeleteBucketTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task DeleteBucket(S3Provider provider, string _, ISimpleClient client)
    {
        Assert.NotEqual(S3Provider.None, provider);
        string tempBucketName = GetTemporaryBucket();

        DeleteBucketResponse deleteResp1 = await client.DeleteBucketAsync(tempBucketName, token: TestContext.Current.CancellationToken);
        Assert.False(deleteResp1.IsSuccess);
        Assert.Equal(ErrorCode.NoSuchBucket, deleteResp1.Error?.Code);

        await client.CreateBucketAsync(tempBucketName, token: TestContext.Current.CancellationToken);

        DeleteBucketResponse deleteResp2 = await client.DeleteBucketAsync(tempBucketName, token: TestContext.Current.CancellationToken);
        Assert.True(deleteResp2.IsSuccess);
        Assert.Equal(204, deleteResp2.StatusCode);

        ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucketName, token: TestContext.Current.CancellationToken);

        if (provider == S3Provider.GoogleCloudStorage && listResp.StatusCode == 200)
        {
            // GCS can return an empty XML listing for a recently deleted bucket rather than NoSuchBucket.
            Assert.Empty(listResp.Objects);
            ListBucketsResponse buckets = await client.ListBucketsAsync(token: TestContext.Current.CancellationToken);
            Assert.True(buckets.IsSuccess);
            Assert.DoesNotContain(buckets.Buckets, item => item.BucketName == tempBucketName);
        }
        else
        {
            Assert.False(listResp.IsSuccess);
            Assert.Equal(404, listResp.StatusCode);
        }
    }
}