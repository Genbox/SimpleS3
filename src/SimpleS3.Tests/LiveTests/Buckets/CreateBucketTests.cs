using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class CreateBucketTests : LiveTestBase
    {
        public CreateBucketTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task CreateStandard()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse pResp = await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateWithCannedAcl()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse pResp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.Acl = BucketCannedAcl.PublicReadWrite).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);

            //TODO: Check ACL once we have that functionality

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateWithCustomAcl()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse pResp = await BucketClient.CreateBucketAsync(tempBucketName, req =>
            {
                req.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                req.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                req.AclGrantRead.AddEmail(TestConstants.TestEmail);
                req.AclGrantWrite.AddEmail(TestConstants.TestEmail);
                req.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
            }).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);

            //TODO: Check ACL once we have that functionality

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateWithObjectLocking()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse pResp = await BucketClient.CreateBucketAsync(tempBucketName, req => req.EnableObjectLocking = true).ConfigureAwait(false);
            Assert.True(pResp.IsSuccess);

            //TODO: Check locking is enabled once we have that functionality

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }
    }
}