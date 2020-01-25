using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Buckets
{
    public class CreateBucketTests : OnlineTestBase
    {
        public CreateBucketTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task CreateBucket()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse resp = await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateBucketCannedAcl()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse resp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.Acl = BucketCannedAcl.PublicReadWrite).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //TODO: Check ACL once we have that functionality

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateBucketCustomAcl()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse resp = await BucketClient.CreateBucketAsync(tempBucketName, req =>
            {
                req.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                req.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                req.AclGrantRead.AddEmail(TestConstants.TestEmail);
                req.AclGrantWrite.AddEmail(TestConstants.TestEmail);
                req.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
            }).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //TODO: Check ACL once we have that functionality

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateBucketObjectLocking()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse resp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.EnableObjectLocking = true).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //TODO: Check locking is enabled once we have that functionality

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }
    }
}