using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketAbacTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketAbacRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        PutBucketAbacResponse put = await client.PutBucketAbacAsync(bucket, true, token: token);
        Assert.Equal(200, put.StatusCode);
        GetBucketAbacResponse read = await client.GetBucketAbacAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.True(read.Enabled);
        Assert.Equal(200, (await client.PutBucketAbacAsync(bucket, false, token: token)).StatusCode);
        GetBucketAbacResponse disabled = await client.GetBucketAbacAsync(bucket, token: token);
        Assert.Equal(200, disabled.StatusCode);
        Assert.False(disabled.Enabled);
    });
}