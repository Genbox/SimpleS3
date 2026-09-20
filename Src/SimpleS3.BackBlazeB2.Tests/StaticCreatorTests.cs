using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.BackBlazeB2;

namespace Genbox.SimpleS3.BackBlazeB2.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        BackBlazeB2Config config = new BackBlazeB2Config("61a951f6abb8", "0000000000000000000000000000000", BackBlazeB2Region.UsWest001);
        using BackBlazeB2Client client = new BackBlazeB2Client(config, driver);

        using GetObjectResponse response = await client.GetObjectAsync("testbucket", "GetObjectAsync", token: TestContext.Current.CancellationToken);
        Assert.Equal("https://testbucket.s3.us-west-001.backblazeb2.com/GetObjectAsync", driver.LastUrl);
    }
}