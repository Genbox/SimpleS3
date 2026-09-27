using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class PutPublicAccessBlockTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutPublicAccessBlockRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        PutPublicAccessBlockResponse resp = await client.PutPublicAccessBlockAsync(tempBucket, request =>
        {
            request.BlockPublicAcls = true;
            request.IgnorePublicAcls = false;
            request.BlockPublicPolicy = true;
            request.RestrictPublicBuckets = false;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, resp.StatusCode);
        GetPublicAccessBlockResponse read = await client.GetPublicAccessBlockAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.True(read.BlockPublicAcls);
        Assert.False(read.IgnorePublicAcls);
        Assert.True(read.BlockPublicPolicy);
        Assert.False(read.RestrictPublicBuckets);
        Assert.Equal(204, (await client.DeletePublicAccessBlockAsync(tempBucket, token: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal(404, (await client.GetPublicAccessBlockAsync(tempBucket, token: TestContext.Current.CancellationToken)).StatusCode);
    });
}