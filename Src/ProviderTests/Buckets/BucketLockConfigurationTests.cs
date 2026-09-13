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

public class BucketLockConfigurationTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task DefaultYearsAndClear(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string retainedKey = "default-years-" + Guid.NewGuid().ToString("N");
        string clearedKey = "cleared-default-" + Guid.NewGuid().ToString("N");
        const string payload = "default-retention-effect";
        string? retainedVersion = null;
        string? clearedVersion = null;

        try
        {
            PutBucketLockConfigurationResponse years = await client.PutBucketLockConfigurationAsync(bucket, true, request =>
            {
                request.LockMode = LockMode.Governance;
                request.LockRetainYears = 1;
            }, token);
            Assert.True(years.IsSuccess, $"{provider} years default: HTTP {years.StatusCode}, {years.Error?.Code}");
            GetBucketLockConfigurationResponse configured = await client.GetBucketLockConfigurationAsync(bucket, token: token);
            Assert.True(configured.IsSuccess);
            Assert.True(configured.Enabled);
            Assert.Equal(LockMode.Governance, configured.LockMode);
            Assert.Null(configured.LockRetainDays);
            Assert.Equal(1, configured.LockRetainYears);

            DateTimeOffset beforeUpload = DateTimeOffset.UtcNow;
            byte[] payloadMd5 = CryptoHelper.Md5Hash(Encoding.UTF8.GetBytes(payload));
            PutObjectResponse retained = await client.PutObjectStringAsync(bucket, retainedKey, payload, config: request => request.ContentMd5 = payloadMd5, token: token);
            Assert.True(retained.IsSuccess);
            Assert.False(string.IsNullOrEmpty(retained.VersionId));
            retainedVersion = retained.VersionId;
            HeadObjectResponse retainedHead = await client.HeadObjectAsync(bucket, retainedKey, request => request.VersionId = retainedVersion, token);
            Assert.True(retainedHead.IsSuccess);
            Assert.Equal(LockMode.Governance, retainedHead.LockMode);
            Assert.NotNull(retainedHead.LockRetainUntil);
            Assert.InRange(retainedHead.LockRetainUntil.Value, beforeUpload.AddDays(364), DateTimeOffset.UtcNow.AddDays(366));
            Assert.Equal(403, (await client.DeleteObjectAsync(bucket, retainedKey, request => request.VersionId = retainedVersion, token)).StatusCode);

            PutBucketLockConfigurationResponse cleared = await client.PutBucketLockConfigurationAsync(bucket, true, token: token);
            Assert.True(cleared.IsSuccess, $"{provider} clear default: HTTP {cleared.StatusCode}, {cleared.Error?.Code}");
            GetBucketLockConfigurationResponse empty = await client.GetBucketLockConfigurationAsync(bucket, token: token);
            Assert.True(empty.IsSuccess);
            Assert.True(empty.Enabled);
            Assert.Equal(LockMode.Unknown, empty.LockMode);
            Assert.Null(empty.LockRetainDays);
            Assert.Null(empty.LockRetainYears);

            PutObjectResponse unretained = await client.PutObjectStringAsync(bucket, clearedKey, payload, config: request => request.ContentMd5 = payloadMd5, token: token);
            Assert.True(unretained.IsSuccess);
            Assert.False(string.IsNullOrEmpty(unretained.VersionId));
            clearedVersion = unretained.VersionId;
            HeadObjectResponse unretainedHead = await client.HeadObjectAsync(bucket, clearedKey, request => request.VersionId = clearedVersion, token);
            Assert.True(unretainedHead.IsSuccess);
            Assert.Equal(LockMode.Unknown, unretainedHead.LockMode);
            Assert.Null(unretainedHead.LockRetainUntil);
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            if (clearedVersion != null)
                Assert.Equal(204, (await client.DeleteObjectAsync(bucket, clearedKey, request => request.VersionId = clearedVersion, cleanup.Token)).StatusCode);

            if (retainedVersion != null)
            {
                DeleteObjectResponse deleted = await client.DeleteObjectAsync(bucket, retainedKey, request =>
                {
                    request.VersionId = retainedVersion;
                    request.BypassGovernanceRetention = true;
                }, cleanup.Token);
                Assert.Equal(204, deleted.StatusCode);
            }

            Assert.Equal(404, (await client.HeadObjectAsync(bucket, retainedKey, token: cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, clearedKey, token: cleanup.Token)).StatusCode);
        }
    }, request => request.EnableObjectLocking = true);

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, LockMode.Compliance, LockMode.Governance)]
    public async Task GetPutBucketLockConfiguration(S3Provider provider, string _, ISimpleClient client, LockMode mode) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        PutBucketLockConfigurationResponse putResp = await client.PutBucketLockConfigurationAsync(tempBucket, true, x =>
        {
            x.LockMode = mode;
            x.LockRetainDays = 2;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(mode, getResp.LockMode);
        Assert.Equal(2, getResp.LockRetainDays);
    }, r => r.EnableObjectLocking = true);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task GetEmptyBucketLock(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
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
        GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(404, getResp.StatusCode);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task OverwriteExistingLock(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        PutBucketLockConfigurationResponse putResp = await client.PutBucketLockConfigurationAsync(tempBucket, true, r =>
        {
            r.LockMode = LockMode.Compliance;
            r.LockRetainDays = 2;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(LockMode.Compliance, getResp.LockMode);
        Assert.Equal(2, getResp.LockRetainDays);

        PutBucketLockConfigurationResponse putResp2 = await client.PutBucketLockConfigurationAsync(tempBucket, true, r =>
        {
            r.LockMode = LockMode.Governance;
            r.LockRetainDays = 5;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp2.StatusCode);

        GetBucketLockConfigurationResponse getResp2 = await client.GetBucketLockConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp2.StatusCode);
        Assert.Equal(LockMode.Governance, getResp2.LockMode);
        Assert.Equal(5, getResp2.LockRetainDays);
    }, r => r.EnableObjectLocking = true);
}