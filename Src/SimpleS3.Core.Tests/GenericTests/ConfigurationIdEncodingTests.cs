using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.GenericS3;
using Genbox.SimpleS3.GenericS3;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ConfigurationIdEncodingTests
{
    [Theory]
    [InlineData("analytics", false)]
    [InlineData("analytics", true)]
    [InlineData("inventory", false)]
    [InlineData("inventory", true)]
    [InlineData("metrics", false)]
    [InlineData("metrics", true)]
    [InlineData("intelligent-tiering", false)]
    [InlineData("intelligent-tiering", true)]
    public async Task ConfigurationIdRemainsOneExactQueryValue(string selector, bool presigned)
    {
        const string id = "folder/a+b &other=value%?é";
        NullNetworkDriver driver = new NullNetworkDriver();
        GenericS3Config config = new GenericS3Config(new StringAccessKey("synthetic-key", "synthetic-secret"), "https://objects.example.test", "region");
        using GenericS3Client client = new GenericS3Client(config, driver);
        string url;

        if (presigned)
        {
            url = selector switch
            {
                "analytics" => client.SignRequest(new DeleteBucketAnalyticsConfigurationRequest("bucket", id), TimeSpan.FromMinutes(1)),
                "inventory" => client.SignRequest(new DeleteBucketInventoryConfigurationRequest("bucket", id), TimeSpan.FromMinutes(1)),
                "metrics" => client.SignRequest(new DeleteBucketMetricsConfigurationRequest("bucket", id), TimeSpan.FromMinutes(1)),
                _ => client.SignRequest(new DeleteBucketIntelligentTieringConfigurationRequest("bucket", id), TimeSpan.FromMinutes(1))
            };
            Assert.Null(driver.LastUrl);
        }
        else
        {
            CancellationToken token = TestContext.Current.CancellationToken;

            switch (selector)
            {
                case "analytics": await client.DeleteBucketAnalyticsConfigurationAsync("bucket", id, token: token); break;
                case "inventory": await client.DeleteBucketInventoryConfigurationAsync("bucket", id, token: token); break;
                case "metrics": await client.DeleteBucketMetricsConfigurationAsync("bucket", id, token: token); break;
                default: await client.DeleteBucketIntelligentTieringConfigurationAsync("bucket", id, token: token); break;
            }

            url = Assert.IsType<string>(driver.LastUrl);
        }

        Uri uri = new Uri(url);
        string[] parameters = uri.Query.TrimStart('?').Split('&');
        string encodedId = Assert.Single(parameters, value => value.StartsWith("id=", StringComparison.Ordinal));
        Assert.Equal("id=" + Uri.EscapeDataString(id), encodedId);
        Assert.Equal(id, Uri.UnescapeDataString(encodedId.Substring(3)));
        Assert.Single(parameters, value => value == selector || value == selector + "=");
        Assert.DoesNotContain(parameters, value => value.StartsWith("other=", StringComparison.Ordinal));
        Assert.Empty(uri.Fragment);
        if (!presigned)
            Assert.Equal(2, parameters.Length);
    }
}