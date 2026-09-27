using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class MultipartMetadataTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task SinglePartETag(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, "etag", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, created.StatusCode);
        byte[] data = new byte[5 * 1024 * 1024];
        data[0] = (byte)'a';
        UploadPartResponse part = await UploadTestPartAsync(client, bucket, "etag", created.UploadId, 1, data);
        Assert.Equal("\"10f74ef02085310ccd1f87150b83e537\"", part.ETag);
        CompleteMultipartUploadResponse complete = await client.CompleteMultipartUploadAsync(bucket, "etag", created.UploadId, [new S3PartInfo(part.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, complete.StatusCode);
        Assert.Equal("\"bd74e21dfa8678d127240f76e518e9c2-1\"", complete.ETag);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task IncompleteUploadAbortHeaders(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule rule = new S3Rule("AbortIncomplete", true) { Filter = new S3Filter(), AbortIncompleteMultipartUploadDays = 1 };
        Assert.True((await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: TestContext.Current.CancellationToken)).IsSuccess);
        await ReadExpectedLifecycleConfigurationAsync(client, bucket, response => response.IsSuccess && response.Rules.Count == 1 && response.Rules[0].AbortIncompleteMultipartUploadDays == 1, TestContext.Current.CancellationToken);
        DateTime expected = DateTime.UtcNow.AddDays(2).Date;
        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, "lifecycle", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, created.StatusCode);
        Assert.Equal(expected, created.AbortsOn!.Value.UtcDateTime.Date);
        Assert.Equal(rule.Id, created.AbortRuleId);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task CompletedObjectExpirationHeaders(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule rule = new S3Rule("ExpireCompleted", true) { Filter = new S3Filter(), Expiration = new S3Expiration(1) };
        Assert.True((await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: TestContext.Current.CancellationToken)).IsSuccess);
        await ReadExpectedLifecycleConfigurationAsync(client, bucket, response => response.IsSuccess && response.Rules.Count == 1 && response.Rules[0].Expiration?.ExpireAfterDays == 1, TestContext.Current.CancellationToken);
        DateTime expected = DateTime.UtcNow.AddDays(2).Date;
        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, "lifecycle", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, created.StatusCode);
        UploadPartResponse part = await UploadTestPartAsync(client, bucket, "lifecycle", created.UploadId, 1, "lifecycle"u8.ToArray());
        CompleteMultipartUploadResponse complete = await client.CompleteMultipartUploadAsync(bucket, "lifecycle", created.UploadId, [new S3PartInfo(part.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, complete.StatusCode);
        Assert.Equal(expected, complete.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
        Assert.Equal(rule.Id, complete.LifeCycleRuleId);
    });
}