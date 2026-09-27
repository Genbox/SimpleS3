using System.Text;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class ObjectLockConfigurationTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2, "inheritance", "deletion-rejected", "clear")]
    public async Task DefaultRetentionBehavior(S3Provider provider, string _, ISimpleClient client, string feature) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string retainedKey = "default-years-" + Guid.NewGuid().ToString("N");
        const string payload = "default-retention-effect";
        PutObjectLockConfigurationResponse years = await client.PutObjectLockConfigurationAsync(bucket, true, request =>
        {
            request.LockMode = LockMode.Governance;
            request.LockRetainYears = 1;
        }, token);
        Assert.True(years.IsSuccess, years.Error?.Message);
        GetObjectLockConfigurationResponse configured = await client.GetObjectLockConfigurationAsync(bucket, token: token);
        Assert.True(configured.IsSuccess);
        Assert.Equal(1, configured.LockRetainYears);

        if (feature == "clear")
        {
            PutObjectLockConfigurationResponse cleared = await client.PutObjectLockConfigurationAsync(bucket, true, token: token);
            Assert.True(cleared.IsSuccess, $"{provider} clear default: HTTP {cleared.StatusCode}, {cleared.Error?.Code}");
            GetObjectLockConfigurationResponse empty = await client.GetObjectLockConfigurationAsync(bucket, token: token);
            Assert.True(empty.IsSuccess);
            Assert.True(empty.Enabled);
            Assert.Equal(LockMode.Unknown, empty.LockMode);
            Assert.Null(empty.LockRetainDays);
            Assert.Null(empty.LockRetainYears);
        }

        DateTimeOffset beforeUpload = DateTimeOffset.UtcNow;
        byte[] payloadMd5 = CryptoHelper.Md5Hash(Encoding.UTF8.GetBytes(payload));
        PutObjectResponse put = await client.PutObjectStringAsync(bucket, retainedKey, payload, config: request => request.ContentMd5 = payloadMd5, token: token);
        Assert.True(put.IsSuccess);
        Assert.False(string.IsNullOrEmpty(put.VersionId));

        if (feature == "deletion-rejected")
        {
            Assert.Equal(403, (await client.DeleteObjectAsync(bucket, retainedKey, request => request.VersionId = put.VersionId, token)).StatusCode);
            return;
        }

        HeadObjectResponse head = await client.HeadObjectAsync(bucket, retainedKey, request => request.VersionId = put.VersionId, token);
        Assert.True(head.IsSuccess);

        if (feature == "clear")
        {
            Assert.Equal(LockMode.Unknown, head.LockMode);
            Assert.Null(head.LockRetainUntil);
        }
        else
        {
            Assert.True(configured.Enabled);
            Assert.Equal(LockMode.Governance, configured.LockMode);
            Assert.Null(configured.LockRetainDays);
            Assert.Equal(LockMode.Governance, head.LockMode);
            Assert.NotNull(head.LockRetainUntil);
            Assert.InRange(head.LockRetainUntil.Value, beforeUpload.AddDays(364), DateTimeOffset.UtcNow.AddDays(366));
        }
    }, request => request.EnableObjectLocking = true);

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, LockMode.Compliance, LockMode.Governance)]
    public async Task PutObjectLockConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client, LockMode mode) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        PutObjectLockConfigurationResponse putResp = await client.PutObjectLockConfigurationAsync(tempBucket, true, x =>
        {
            x.LockMode = mode;
            x.LockRetainDays = 2;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        GetObjectLockConfigurationResponse getResp = await client.GetObjectLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(mode, getResp.LockMode);
        Assert.Equal(2, getResp.LockRetainDays);
    }, r => r.EnableObjectLocking = true);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task GetEmptyBucketLock(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        GetObjectLockConfigurationResponse getResp = await client.GetObjectLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.True(getResp.Enabled);
        Assert.Equal(LockMode.Unknown, getResp.LockMode);
        Assert.Null(getResp.LockRetainDays);
        Assert.Null(getResp.LockRetainYears);
    }, r => r.EnableObjectLocking = true);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task GetWhenBucketLockIsDisabled(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        GetObjectLockConfigurationResponse getResp = await client.GetObjectLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(404, getResp.StatusCode);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task OverwriteExistingLock(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        PutObjectLockConfigurationResponse putResp = await client.PutObjectLockConfigurationAsync(tempBucket, true, r =>
        {
            r.LockMode = LockMode.Compliance;
            r.LockRetainDays = 2;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        GetObjectLockConfigurationResponse getResp = await client.GetObjectLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(LockMode.Compliance, getResp.LockMode);
        Assert.Equal(2, getResp.LockRetainDays);

        PutObjectLockConfigurationResponse putResp2 = await client.PutObjectLockConfigurationAsync(tempBucket, true, r =>
        {
            r.LockMode = LockMode.Governance;
            r.LockRetainDays = 5;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp2.StatusCode);

        GetObjectLockConfigurationResponse getResp2 = await client.GetObjectLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp2.StatusCode);
        Assert.Equal(LockMode.Governance, getResp2.LockMode);
        Assert.Equal(5, getResp2.LockRetainDays);
    }, r => r.EnableObjectLocking = true);
}