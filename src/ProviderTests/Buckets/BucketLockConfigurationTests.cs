using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class BucketLockConfigurationTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All, LockMode.Compliance, LockMode.Governance)]
        public async Task GetPutBucketLockConfiguration(S3Provider provider, IProfile _, ISimpleClient client, LockMode mode)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                PutBucketLockConfigurationResponse putResp = await client.PutBucketLockConfigurationAsync(tempBucket, true, x =>
                {
                    x.LockMode = mode;
                    x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2);
                }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);

                Assert.Equal(mode, getResp.LockMode);
                Assert.Equal(DateTimeOffset.UtcNow.AddDays(2 - 1).DateTime, getResp.LockRetainUntil!.Value.DateTime, TimeSpan.FromMinutes(1));

            }, req => req.EnableObjectLocking = true).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetEmptyBucketLock(S3Provider provider, IProfile _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);

                Assert.Equal(LockMode.Unknown, getResp.LockMode);
                Assert.Null(getResp.LockRetainUntil);

            }, req => req.EnableObjectLocking = true).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetWhenBucketLockIsDisabled(S3Provider provider, IProfile _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
                Assert.Equal(404, getResp.StatusCode);
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task OverwriteExistingLock(S3Provider provider, IProfile _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                PutBucketLockConfigurationResponse putResp = await client.PutBucketLockConfigurationAsync(tempBucket, true, x =>
                {
                    x.LockMode = LockMode.Compliance;
                    x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2);
                }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLockConfigurationResponse getResp = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);
                Assert.Equal(LockMode.Compliance, getResp.LockMode);
                Assert.Equal(DateTimeOffset.UtcNow.AddDays(2 - 1).DateTime, getResp.LockRetainUntil!.Value.DateTime, TimeSpan.FromMinutes(1));

                PutBucketLockConfigurationResponse putResp2 = await client.PutBucketLockConfigurationAsync(tempBucket, true, x =>
                {
                    x.LockMode = LockMode.Governance;
                    x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(5);
                }).ConfigureAwait(false);
                Assert.True(putResp2.IsSuccess);

                GetBucketLockConfigurationResponse getResp2 = await client.GetBucketLockConfigurationAsync(tempBucket).ConfigureAwait(false);
                Assert.True(getResp2.IsSuccess);
                Assert.Equal(LockMode.Governance, getResp2.LockMode);
                Assert.Equal(DateTimeOffset.UtcNow.AddDays(5 - 1).DateTime, getResp2.LockRetainUntil!.Value.DateTime, TimeSpan.FromMinutes(1));

            }).ConfigureAwait(false);
        }
    }
}