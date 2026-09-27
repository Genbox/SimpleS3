using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketLocationTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task GetBucketLocationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        GetBucketLocationResponse response = await client.GetBucketLocationAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        string expected = ProviderSetup.Instance.GetProfile(provider).RegionCode;
        string? location = response.Location;
        if (provider == S3Provider.AmazonS3)
            location = location switch { null or "" => "us-east-1", "EU" => "eu-west-1", _ => location };
        Assert.Equal(expected, location, StringComparer.OrdinalIgnoreCase);
    });
}