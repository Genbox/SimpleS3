using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets;

public class ListBucketsTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListBuckets(S3Provider _, string bucket, ISimpleClient client)
    {
        ListBucketsResponse listResp = await client.ListBucketsAsync().ConfigureAwait(false);
        Assert.True(listResp.Buckets.Count > 0);
        Assert.Single(listResp.Buckets, x => x.BucketName == bucket);
    }
}