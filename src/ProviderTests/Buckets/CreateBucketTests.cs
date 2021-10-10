using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class CreateBucketTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task CreateBucket(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            string tempBucketName = GetTemporaryBucket();

            CreateBucketResponse resp = await client.CreateBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //Delete again to cleanup
            await client.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task CreateBucketCannedAcl(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async s =>
            {
                //TODO: Check ACL once we have that functionality
            }, req => req.Acl = BucketCannedAcl.PublicReadWrite);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task CreateBucketCustomAcl(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async s =>
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

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task CreateBucketObjectLocking(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async s =>
            {
                //TODO: Check locking is enabled once we have that functionality
            }, req => req.EnableObjectLocking = true);
        }
    }
}