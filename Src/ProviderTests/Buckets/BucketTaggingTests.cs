using Genbox.ProviderTests.Code;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketTaggingTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetPutDeleteBucketTagging(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("MyKey", "MyValue");
            tags.Add("MyKey2", "MyValue2");

            PutBucketTaggingResponse putResp = await client.PutBucketTaggingAsync(tempBucket, tags);
            Assert.True(putResp.IsSuccess);

            GetBucketTaggingResponse getResp = await client.GetBucketTaggingAsync(tempBucket);
            Assert.True(getResp.IsSuccess);

            Assert.Equal(tags, getResp.Tags);

            DeleteBucketTaggingResponse deleteResp = await client.DeleteBucketTaggingAsync(tempBucket);
            Assert.True(deleteResp.IsSuccess);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetEmptyBucketTagging(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            GetBucketTaggingResponse getResp = await client.GetBucketTaggingAsync(tempBucket);
            Assert.Equal(404, getResp.StatusCode);
        });
    }
}