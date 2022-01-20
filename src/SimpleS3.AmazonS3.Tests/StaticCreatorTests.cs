using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Xunit;

namespace Genbox.SimpleS3.AmazonS3.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        AmazonS3Config config = new AmazonS3Config();
        config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
        config.Region = AmazonS3Region.UsEast1;

        using AmazonS3Client client = new AmazonS3Client(config, driver);

        await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
        Assert.Equal("https://testbucket.s3.us-east-1.amazonaws.com/GetObjectAsync", driver.LastUrl);
    }
}