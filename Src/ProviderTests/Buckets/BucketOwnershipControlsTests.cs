using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketOwnershipControlsTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, ObjectOwnership.BucketOwnerPreferred, ObjectOwnership.ObjectWriter)]
    public async Task PutBucketOwnershipControlsRoundTrip(S3Provider provider, string _, ISimpleClient client, ObjectOwnership ownership) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutBucketOwnershipControlsAsync(bucket, [new S3OwnershipControlsRule(ownership)], token: token)).StatusCode);
        GetBucketOwnershipControlsResponse read = await client.GetBucketOwnershipControlsAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(ownership, Assert.Single(read.Rules).ObjectOwnership);
        Assert.Equal(204, (await client.DeleteBucketOwnershipControlsAsync(bucket, token: token)).StatusCode);
        Assert.Equal(404, (await client.GetBucketOwnershipControlsAsync(bucket, token: token)).StatusCode);
    });
}