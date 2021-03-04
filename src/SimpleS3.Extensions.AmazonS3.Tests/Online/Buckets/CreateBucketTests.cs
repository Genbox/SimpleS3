using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Buckets
{
    public class CreateBucketTests : AwsTestBase
    {
        public CreateBucketTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task CreateBucket()
        {
            string tempBucketName = GetTempBucketName();

            CreateBucketResponse resp = await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //Delete again to cleanup
            await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Fact]
        public async Task CreateBucketCannedAcl()
        {
            await CreateTempBucketAsync(async s =>
            {
                //TODO: Check ACL once we have that functionality
            }, req => req.Acl = BucketCannedAcl.PublicReadWrite);
        }

        [Fact]
        public async Task CreateBucketCustomAcl()
        {
            await CreateTempBucketAsync(async s =>
            {
                //TODO: Check ACL once we have that functionality
            }, req =>
            {
                req.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                req.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                req.AclGrantRead.AddEmail(TestConstants.TestEmail);
                req.AclGrantWrite.AddEmail(TestConstants.TestEmail);
                req.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
            });
        }

        [Fact]
        public async Task CreateBucketObjectLocking()
        {
            await CreateTempBucketAsync(async s =>
            {
                //TODO: Check locking is enabled once we have that functionality
            }, req => req.EnableObjectLocking = true);
        }
    }
}