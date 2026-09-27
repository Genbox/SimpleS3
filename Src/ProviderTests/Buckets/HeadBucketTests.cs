using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class HeadBucketTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task HeadBucketRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        HeadBucketResponse headResp = await client.HeadBucketAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, headResp.StatusCode);
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task MissingBucket(S3Provider _1, string _2, ISimpleClient client)
    {
        HeadBucketResponse response = await client.HeadBucketAsync(GetTemporaryBucket(), token: TestContext.Current.CancellationToken);
        Assert.Equal(404, response.StatusCode);
    }
}