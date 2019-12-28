using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.LiveTests.Objects
{
    public class ObjectAclTests : LiveTestBase
    {
        public ObjectAclTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task PutGetObjectAcl()
        {
            string objectKey = nameof(PutGetObjectAcl);

            //Create an object
            await UploadAsync(objectKey).ConfigureAwait(false);

            //Get the ACL, which should be the default one (owner has ACL)
            GetObjectAclResponse getResp = await ObjectClient.GetObjectAclAsync(BucketName, objectKey).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);

            S3Grant grant = Assert.Single(getResp.Grants);
            Assert.Equal(TestConstants.TestUserId, grant.Id);
            Assert.Equal(TestConstants.TestUsername, grant.Name);
            Assert.Equal(Permission.FullControl, grant.Permission);
            Assert.Equal(GrantType.User, grant.Type);

            //Update the object to have another ACL using Canned ACLs
            PutObjectAclResponse putResp = await ObjectClient.PutObjectAclAsync(BucketName, objectKey, req => req.Acl = ObjectCannedAcl.PublicReadWrite).ConfigureAwait(false);
            Assert.True(putResp.IsSuccess);

            GetObjectAclResponse getResp2 = await ObjectClient.GetObjectAclAsync(BucketName, objectKey).ConfigureAwait(false);
            Assert.True(getResp2.IsSuccess);

            Assert.Equal(TestConstants.TestUserId, getResp2.Owner.Id);
            Assert.Equal(TestConstants.TestUsername, getResp2.Owner.Name);

            Assert.Equal(3, getResp2.Grants.Count);

            //This is the default owner ACL
            S3Grant first = getResp2.Grants[0];
            Assert.Equal(TestConstants.TestUserId, first.Id);
            Assert.Equal(TestConstants.TestUsername, first.Name);
            Assert.Equal(Permission.FullControl, first.Permission);
            Assert.Equal(GrantType.User, first.Type);

            //Next 2 ACLs should be READ + WRITE for AllUsers
            S3Grant second = getResp2.Grants[1];
            Assert.Equal("http://acs.amazonaws.com/groups/global/AllUsers", second.Uri);
            Assert.Equal(Permission.Read, second.Permission);
            Assert.Equal(GrantType.Group, second.Type);

            S3Grant third = getResp2.Grants[2];
            Assert.Equal("http://acs.amazonaws.com/groups/global/AllUsers", third.Uri);
            Assert.Equal(Permission.Write, third.Permission);
            Assert.Equal(GrantType.Group, third.Type);
        }
    }
}
