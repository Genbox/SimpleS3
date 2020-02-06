using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Buckets
{
    public class BucketLockConfigurationTests : OnlineTestBase
    {
        public BucketLockConfigurationTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData(LockMode.Compliance)]
        [InlineData(LockMode.Governance)]
        public async Task GetPutBucketLockConfiguration(LockMode mode)
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse createResp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.EnableObjectLocking = true).ConfigureAwait(false);
            Assert.True(createResp.IsSuccess);

            PutBucketLockConfigurationResponse putResp = await BucketClient.PutBucketLockConfigurationAsync(tempBucketName, true, x =>
            {
                x.LockMode = mode;
                x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2);
            }).ConfigureAwait(false);
            Assert.True(putResp.IsSuccess);

            GetBucketLockConfigurationResponse getResp = await BucketClient.GetBucketLockConfigurationAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);

            Assert.Equal(mode, getResp.LockMode);
            Assert.Equal(DateTimeOffset.UtcNow.AddDays(2 - 1).DateTime, getResp.LockRetainUntil.Value.DateTime, TimeSpan.FromMinutes(1));

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetEmptyBucketLock()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse createResp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.EnableObjectLocking = true).ConfigureAwait(false);
            Assert.True(createResp.IsSuccess);

            GetBucketLockConfigurationResponse getResp = await BucketClient.GetBucketLockConfigurationAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);

            Assert.Equal(LockMode.Unknown, getResp.LockMode);
            Assert.Null(getResp.LockRetainUntil);

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetWhenBucketLockIsDisabled()
        {
            await CreateTempBucketAsync(async x =>
            {
                GetBucketLockConfigurationResponse getResp = await BucketClient.GetBucketLockConfigurationAsync(x).ConfigureAwait(false);
                Assert.Equal(404, getResp.StatusCode);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task OverwriteExistingLock()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse createResp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.EnableObjectLocking = true).ConfigureAwait(false);
            Assert.True(createResp.IsSuccess);

            PutBucketLockConfigurationResponse putResp = await BucketClient.PutBucketLockConfigurationAsync(tempBucketName, true, x =>
            {
                x.LockMode = LockMode.Compliance;
                x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2);
            }).ConfigureAwait(false);
            Assert.True(putResp.IsSuccess);

            GetBucketLockConfigurationResponse getResp = await BucketClient.GetBucketLockConfigurationAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);
            Assert.Equal(LockMode.Compliance, getResp.LockMode);
            Assert.Equal(DateTimeOffset.UtcNow.AddDays(2 - 1).DateTime, getResp.LockRetainUntil.Value.DateTime, TimeSpan.FromMinutes(1));

            PutBucketLockConfigurationResponse putResp2 = await BucketClient.PutBucketLockConfigurationAsync(tempBucketName, true, x =>
                 {
                     x.LockMode = LockMode.Governance;
                     x.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(5);
                 }).ConfigureAwait(false);
            Assert.True(putResp2.IsSuccess);

            GetBucketLockConfigurationResponse getResp2 = await BucketClient.GetBucketLockConfigurationAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(getResp2.IsSuccess);
            Assert.Equal(LockMode.Governance, getResp2.LockMode);
            Assert.Equal(DateTimeOffset.UtcNow.AddDays(5 - 1).DateTime, getResp2.LockRetainUntil.Value.DateTime, TimeSpan.FromMinutes(1));

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }
    }
}