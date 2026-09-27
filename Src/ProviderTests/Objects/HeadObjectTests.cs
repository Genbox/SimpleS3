using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class HeadObjectTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task HeadObjectRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(HeadObjectRoundTrip) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, "head content", token: TestContext.Current.CancellationToken), async put =>
        {
            HeadObjectResponse head = await client.HeadObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, head.StatusCode);
            Assert.Equal(12, head.ContentLength);
            Assert.Equal(put.ETag, head.ETag);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task HeadObjectLifecycle(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(HeadObjectLifecycle) + Guid.NewGuid().ToString("N");
        DateTime expected = DateTime.UtcNow.AddDays(2).Date;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), async _ =>
        {
            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, headResp.StatusCode);

            //Expiration should work on head too
            Assert.Equal(expected, headResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", headResp.LifeCycleRuleId);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task HeadObjectMultipleTags(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(HeadObjectMultipleTags) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r =>
        {
            r.Tags.Add("mykey1", "myvalue1");
            r.Tags.Add("mykey2", "myvalue2");
        }, TestContext.Current.CancellationToken), async _ =>
        {
            HeadObjectResponse response = await client.HeadObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(2, response.TagCount);
        });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2, "cache-control", "disposition", "encoding", "language", "type", "expires")]
    public async Task HeadObjectResponseHeaders(S3Provider _, string bucket, ISimpleClient client, string header)
    {
        string key = nameof(HeadObjectResponseHeaders) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), async _ =>
        {
            DateTimeOffset expires = DateTimeOffset.UtcNow.AddHours(1);
            HeadObjectResponse response = await client.HeadObjectAsync(bucket, key, request => ConfigureResponseHeaderOverride(request, header, expires), TestContext.Current.CancellationToken);
            AssertResponseHeaderOverride(response, header, expires);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task HeadObjectWebsiteRedirect(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(HeadObjectWebsiteRedirect) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r => r.WebsiteRedirectLocation = "https://google.com", TestContext.Current.CancellationToken), async _ =>
        {
            HeadObjectResponse response = await client.HeadObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("https://google.com", response.WebsiteRedirectLocation);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    [SuppressMessage("Security", "CA5350", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
    public async Task HeadObjectChecksum(S3Provider _, string bucket, ISimpleClient client)
    {
        byte[] data = [1, 2, 3, 4];
        byte[] checksum = SHA1.HashData(data);

        string key = nameof(HeadObjectChecksum) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectDataAsync(bucket, key, data, r =>
        {
            r.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
            r.Checksum = checksum;
        }, TestContext.Current.CancellationToken), async put =>
        {
            Assert.Equal(checksum, put.Checksum);
            Assert.Equal(ChecksumType.FullObject, put.ChecksumType);
            HeadObjectResponse response = await client.HeadObjectAsync(bucket, key, r => r.EnableChecksum = true, TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ChecksumType.FullObject, response.ChecksumType);
            Assert.Equal(ChecksumAlgorithm.Sha1, response.ChecksumAlgorithm);
            Assert.Equal(checksum, response.Checksum);
        });
    }
}