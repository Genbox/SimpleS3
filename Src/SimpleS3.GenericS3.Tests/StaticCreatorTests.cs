using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.GenericS3;

namespace Genbox.SimpleS3.GenericS3.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        GenericS3Config config = new GenericS3Config();
        config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
        config.Endpoint = "https://myendpoint.com";
        config.RegionCode = "us-east-1";

        using GenericS3Client client = new GenericS3Client(config, driver);

        await client.GetObjectAsync("testbucket", "GetObjectAsync");
        Assert.Equal("https://myendpoint.com/GetObjectAsync", driver.LastUrl);
    }
}