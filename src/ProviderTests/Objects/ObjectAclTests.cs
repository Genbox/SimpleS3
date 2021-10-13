using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class ObjectAclTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutGetObjectAcl(S3Provider provider, string bucket, ISimpleClient client)
        {
            string objectKey = nameof(PutGetObjectAcl);

            //Create an object
            PutObjectResponse putResp1 = await client.PutObjectAsync(bucket, objectKey, null).ConfigureAwait(false);
            Assert.Equal(200, putResp1.StatusCode);

            //Get the ACL, which should be the default one (owner has ACL)
            GetObjectAclResponse getResp = await client.GetObjectAclAsync(bucket, objectKey).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);

            S3Grant? grant = Assert.Single(getResp.Grants);
            Assert.Equal(S3Permission.FullControl, grant.Permission);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(GrantType.CanonicalUser, grant.Grantee.Type);
                Assert.Equal(TestConstants.TestUserId, grant.Grantee.Id);
                Assert.Equal(TestConstants.TestUsername, grant.Grantee.DisplayName);
            }

            //Update the object to have another ACL using Canned ACLs
            PutObjectAclResponse putResp2 = await client.PutObjectAclAsync(bucket, objectKey, r => r.Acl = ObjectCannedAcl.PublicRead).ConfigureAwait(false);
            Assert.Equal(200, putResp2.StatusCode);

            GetObjectAclResponse getResp2 = await client.GetObjectAclAsync(bucket, objectKey).ConfigureAwait(false);
            Assert.Equal(200, getResp2.StatusCode);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, getResp2.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, getResp2.Owner.Name);
            }

            Assert.Equal(3, getResp2.Grants.Count);

            //This is the default owner ACL
            S3Grant first = getResp2.Grants[0];

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, first.Grantee.Id);
                Assert.Equal(TestConstants.TestUsername, first.Grantee.DisplayName);
            }

            Assert.Equal(S3Permission.FullControl, first.Permission);
            Assert.Equal(GrantType.CanonicalUser, first.Grantee.Type);

            //Next 2 ACLs should be READ + WRITE for AllUsers
            S3Grant second = getResp2.Grants[1];
            Assert.Equal("http://acs.amazonaws.com/groups/global/AllUsers", second.Grantee.Uri);
            Assert.Equal(S3Permission.Read, second.Permission);
            Assert.Equal(GrantType.Group, second.Grantee.Type);

            S3Grant third = getResp2.Grants[2];
            Assert.Equal("http://acs.amazonaws.com/groups/global/AllUsers", third.Grantee.Uri);
            Assert.Equal(S3Permission.Write, third.Permission);
            Assert.Equal(GrantType.Group, third.Grantee.Type);
        }
    }
}