using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class CreateBucketTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task CreateBucketRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        HeadBucketResponse response = await client.HeadBucketAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
    });
}