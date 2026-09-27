using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketAclTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketAclRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutPublicAccessBlockAsync(bucket, request =>
        {
            request.BlockPublicAcls = false;
            request.IgnorePublicAcls = false;
            request.BlockPublicPolicy = true;
            request.RestrictPublicBuckets = true;
        }, token)).StatusCode);
        GetBucketAclResponse initial = await client.GetBucketAclAsync(bucket, token: token);
        Assert.Equal(200, initial.StatusCode);
        AssertOwnerGrant(initial.Owner, Assert.Single(initial.Grants));

        Assert.Equal(200, (await client.PutBucketAclAsync(bucket, request => request.Acl = BucketCannedAcl.AuthenticatedRead, token)).StatusCode);
        GetBucketAclResponse read = await client.GetBucketAclAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(2, read.Grants.Count);
        AssertOwnerGrant(read.Owner, Assert.Single(read.Grants, grant => grant.Permission == S3Permission.FullControl));
        S3Grant group = Assert.Single(read.Grants, grant => grant.Permission == S3Permission.Read);
        Assert.Equal(GrantType.Group, group.Grantee.Type);
        Assert.Equal("http://acs.amazonaws.com/groups/global/AuthenticatedUsers", group.Grantee.Uri);
    }, request => request.ObjectOwnership = ObjectOwnership.ObjectWriter);
}