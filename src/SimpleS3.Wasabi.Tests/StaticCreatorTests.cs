using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.Wasabi;
using Xunit;

namespace Genbox.SimpleS3.Wasabi.Tests
{
    public class StaticCreatorTests
    {
        [Fact]
        public async Task StaticClient()
        {
            FakeNetworkDriver driver = new FakeNetworkDriver();

            WasabiConfig config = new WasabiConfig();
            config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
            config.Region = WasabiRegion.EuCentral1;

            using WasabiClient client = new WasabiClient(config, driver);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.eu-central-1.wasabisys.com/GetObjectAsync", driver.SendResource);
        }
    }
}