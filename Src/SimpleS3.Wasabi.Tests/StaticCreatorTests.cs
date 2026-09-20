using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.Wasabi;

namespace Genbox.SimpleS3.Wasabi.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        WasabiConfig config = new WasabiConfig("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY", WasabiRegion.EuCentral1);
        using WasabiClient client = new WasabiClient(config, driver);

        using GetObjectResponse response = await client.GetObjectAsync("testbucket", "GetObjectAsync", token: TestContext.Current.CancellationToken);
        Assert.Equal("https://testbucket.s3.eu-central-1.wasabisys.com/GetObjectAsync", driver.LastUrl);
    }
}