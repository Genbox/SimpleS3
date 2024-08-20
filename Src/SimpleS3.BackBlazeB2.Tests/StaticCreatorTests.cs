using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.BackBlazeB2;

namespace Genbox.SimpleS3.BackBlazeB2.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        BackBlazeB2Config config = new BackBlazeB2Config();
        config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
        config.Region = BackBlazeB2Region.UsWest001;

        using BackBlazeB2Client client = new BackBlazeB2Client(config, driver);

        await client.GetObjectAsync("testbucket", "GetObjectAsync");
        Assert.Equal("https://testbucket.s3.us-west-001.backblazeb2.com/GetObjectAsync", driver.LastUrl);
    }
}