using Genbox.ProviderTests.Code;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class CreateBucketTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task CreateBucket(S3Provider _, string __, ISimpleClient client)
    {
        string tempBucketName = GetTemporaryBucket();

        CreateBucketResponse resp = await client.CreateBucketAsync(tempBucketName);
        Assert.True(resp.IsSuccess);

        //Delete again to cleanup
        await client.DeleteBucketAsync(tempBucketName);
    }

    //[Theory]
    //[MultipleProviders(S3Provider.All)]
    //public async Task CreateBucketCannedAcl(S3Provider provider, string _, ISimpleClient client)
    //{
    //    await CreateTempBucketAsync(provider, client, async tempBucket =>
    //    {
    //        //TODO: Check ACL once we have that functionality
    //    }, r => r.Acl = BucketCannedAcl.Private);
    //}

    //[Theory]
    //[MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    //public async Task CreateBucketCustomAcl(S3Provider provider, string _, ISimpleClient client)
    //{
    //    await CreateTempBucketAsync(provider, client, async tempBucket =>
    //    {
    //        //TODO: Check ACL once we have that functionality
    //    }, r =>
    //    {
    //        r.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
    //        r.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
    //        r.AclGrantRead.AddEmail(TestConstants.TestEmail);
    //        r.AclGrantWrite.AddEmail(TestConstants.TestEmail);
    //        r.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
    //    });
    //}

    //[Theory]
    //[MultipleProviders(S3Provider.All)]
    //public async Task CreateBucketObjectLocking(S3Provider provider, string _, ISimpleClient client)
    //{
    //    await CreateTempBucketAsync(provider, client, async tempBucket =>
    //    {
    //        //TODO: Check locking is enabled once we have that functionality
    //    }, r => r.EnableObjectLocking = true);
    //}
}