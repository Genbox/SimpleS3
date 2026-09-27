using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketReplicationTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task DeleteAbsentReplicationConfiguration(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;

        // An unconfigured bucket exercises idempotent deletion without a replication IAM role.
        Assert.Equal(204, (await client.DeleteBucketReplicationAsync(bucket, token: token)).StatusCode);
        Assert.Equal(204, (await client.DeleteBucketReplicationAsync(bucket, token: token)).StatusCode);
    });
}