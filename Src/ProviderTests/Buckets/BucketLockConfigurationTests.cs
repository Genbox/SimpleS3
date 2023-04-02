using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketLockConfigurationTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3, LockMode.Compliance, LockMode.Governance)]
    public async Task GetPutBucketLockConfiguration(S3Provider provider, string _, ISimpleClient client, LockMode mode)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            PutBucketLockConfigurationResponse putResp = await client.PutBucketLockConfigurationAsync(tempBucket, true, x =>
            {
                x.LockMode = mode;
                x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2);
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(mode, getResp.LockMode);
            Assert.Equal(DateTimeOffset.UtcNow.AddDays(2 - 1).DateTime, getResp.LockRetainUntil!.Value.DateTime, TimeSpan.FromMinutes(1));
        }, r => r.EnableObjectLocking = true).ConfigureAwait(false);
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task GetEmptyBucketLock(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(LockMode.Unknown, getResp.LockMode);
            Assert.Null(getResp.LockRetainUntil);
        }, r => r.EnableObjectLocking = true).ConfigureAwait(false);
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task GetWhenBucketLockIsDisabled(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
            Assert.Equal(404, getResp.StatusCode);
        }).ConfigureAwait(false);
    }

    [Theory(Skip = "seem to fail on all platforms right now")]
    [MultipleProviders(S3Provider.All)]
    public async Task OverwriteExistingLock(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            PutBucketLockConfigurationResponse putResp = await client.PutBucketLockConfigurationAsync(tempBucket, true, r =>
            {
                r.LockMode = LockMode.Compliance;
                r.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2);
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(LockMode.Compliance, getResp.LockMode);
            Assert.Equal(DateTimeOffset.UtcNow.AddDays(2 - 1).DateTime, getResp.LockRetainUntil!.Value.DateTime, TimeSpan.FromMinutes(1));

            PutBucketLockConfigurationResponse putResp2 = await client.PutBucketLockConfigurationAsync(tempBucket, true, r =>
            {
                r.LockMode = LockMode.Governance;
                r.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(5);
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp2.StatusCode);

            GetBucketLockConfigurationResponse getResp2 = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
            Assert.Equal(200, getResp2.StatusCode);
            Assert.Equal(LockMode.Governance, getResp2.LockMode);
            Assert.Equal(DateTimeOffset.UtcNow.AddDays(5 - 1).DateTime, getResp2.LockRetainUntil!.Value.DateTime, TimeSpan.FromMinutes(1));
        }).ConfigureAwait(false);
    }
}