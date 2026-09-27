using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketTaggingTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutBucketTaggingRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        Dictionary<string, string> tags = new Dictionary<string, string>(StringComparer.Ordinal);
        tags.Add("MyKey", "MyValue");
        tags.Add("MyKey2", "MyValue2");

        PutBucketTaggingResponse putResp = await client.PutBucketTaggingAsync(tempBucket, tags, token: TestContext.Current.CancellationToken);
        Assert.True(putResp.IsSuccess);

        GetBucketTaggingResponse getResp = await client.GetBucketTaggingAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.True(getResp.IsSuccess);

        Assert.Equal(tags, getResp.Tags);

        DeleteBucketTaggingResponse deleteResp = await client.DeleteBucketTaggingAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.True(deleteResp.IsSuccess);
        Assert.Equal(404, (await client.GetBucketTaggingAsync(tempBucket, token: TestContext.Current.CancellationToken)).StatusCode);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetEmptyBucketTagging(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        GetBucketTaggingResponse getResp = await client.GetBucketTaggingAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(404, getResp.StatusCode);
    });
}