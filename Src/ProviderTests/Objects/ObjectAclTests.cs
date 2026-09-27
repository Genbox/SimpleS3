using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectAclTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task PutObjectAclRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(PutObjectAclRoundTrip) + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, objectKey, () => client.PutObjectAsync(bucket, objectKey, null, token: TestContext.Current.CancellationToken), async _ =>
        {
            //Update the object to have another ACL using Canned ACLs
            PutObjectAclResponse updated = await client.PutObjectAclAsync(bucket, objectKey, r => r.Acl = ObjectCannedAcl.PublicRead, TestContext.Current.CancellationToken);
            Assert.Equal(200, updated.StatusCode);

            GetObjectAclResponse acl = await client.GetObjectAclAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, acl.StatusCode);
            Assert.Equal(2, acl.Grants.Count);

            //This is the default owner ACL
            S3Grant ownerGrant = Assert.Single(acl.Grants, grant => grant.Permission == S3Permission.FullControl);
            AssertOwnerGrant(acl.Owner, ownerGrant);
            Assert.Null(ownerGrant.Grantee.DisplayName);

            S3Grant publicGrant = Assert.Single(acl.Grants, grant => grant.Permission == S3Permission.Read);
            Assert.Equal("http://acs.amazonaws.com/groups/global/AllUsers", publicGrant.Grantee.Uri);
            Assert.Equal(S3Permission.Read, publicGrant.Permission);
            Assert.Equal(GrantType.Group, publicGrant.Grantee.Type);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task DefaultAcl(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(DefaultAcl) + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), async _ =>
        {
            GetObjectAclResponse response = await client.GetObjectAclAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            S3Grant grant = Assert.Single(response.Grants);
            AssertOwnerGrant(response.Owner, grant);
            Assert.Null(grant.Grantee.DisplayName);
        });
    }
}