using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.Wasabi;

namespace Genbox.SimpleS3.Wasabi.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        WasabiConfig config = new WasabiConfig("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY", WasabiRegion.EuCentral1);
        using WasabiClient client = new WasabiClient(config, driver);

        await client.GetObjectAsync("testbucket", "GetObjectAsync");
        Assert.Equal("https://testbucket.s3.eu-central-1.wasabisys.com/GetObjectAsync", driver.LastUrl);
    }
}