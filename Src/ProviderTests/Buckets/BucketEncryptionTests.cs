using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketEncryptionTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketEncryptionRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutBucketEncryptionAsync(bucket, [new S3ServerSideEncryptionRule(SseAlgorithm.AwsKms)], token: token)).StatusCode);
        GetBucketEncryptionResponse read = await client.GetBucketEncryptionAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(SseAlgorithm.AwsKms, Assert.Single(read.Rules).SseAlgorithm);
        Assert.Equal(204, (await client.DeleteBucketEncryptionAsync(bucket, token: token)).StatusCode);
        GetBucketEncryptionResponse deleted = await client.GetBucketEncryptionAsync(bucket, token: token);
        Assert.Equal(200, deleted.StatusCode);

        // S3 retains its automatic SSE-S3 baseline after deleting an explicit default.
        Assert.Equal(SseAlgorithm.Aes256, Assert.Single(deleted.Rules).SseAlgorithm);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, false, true)]
    public async Task BucketKeySetting(S3Provider provider, string _, ISimpleClient client, bool enabled) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(SseAlgorithm.AwsKms) { BucketKeyEnabled = enabled };
        Assert.Equal(200, (await client.PutBucketEncryptionAsync(bucket, [rule], token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketEncryptionResponse read = await client.GetBucketEncryptionAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(enabled, Assert.Single(read.Rules).BucketKeyEnabled);
    });
}