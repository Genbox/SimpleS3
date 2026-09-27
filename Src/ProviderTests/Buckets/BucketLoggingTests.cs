using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketLoggingTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketLoggingRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        await EnableLoggingAsync(client, bucket);
        GetBucketLoggingResponse read = await client.GetBucketLoggingAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.NotNull(read.LoggingConfiguration);
        Assert.Equal(bucket, read.LoggingConfiguration.TargetBucket);
        Assert.Equal("access-logs/", read.LoggingConfiguration.TargetPrefix);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task DisableLogging(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        await EnableLoggingAsync(client, bucket);
        Assert.Equal(200, (await client.PutBucketLoggingAsync(bucket, null, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketLoggingResponse read = await client.GetBucketLoggingAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.Null(read.LoggingConfiguration);
    });

    private static async Task EnableLoggingAsync(ISimpleClient client, string bucket)
    {
        // The disposable bucket also owns its log destination; no external bucket or account ID is needed.
        string policy = $$"""
                          {"Version":"2012-10-17","Statement":[{"Effect":"Allow","Principal":{"Service":"logging.s3.amazonaws.com"},"Action":"s3:PutObject","Resource":"arn:aws:s3:::{{bucket}}/access-logs/*"}]}
                          """;
        Assert.Equal(204, (await client.PutBucketPolicyAsync(bucket, policy, token: TestContext.Current.CancellationToken)).StatusCode);
        PutBucketLoggingResponse response = await client.PutBucketLoggingAsync(bucket, new S3LoggingConfiguration(bucket, "access-logs/"), token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
    }
}