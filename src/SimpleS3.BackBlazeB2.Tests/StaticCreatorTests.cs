#if COMMERCIAL
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.BackBlazeB2;
using Xunit;

namespace Genbox.SimpleS3.BackBlazeB2.Tests
{
    public class StaticCreatorTests
    {
        [Fact]
        public async Task StaticClient()
        {
            FakeNetworkDriver driver = new FakeNetworkDriver();

            BackBlazeB2Config config = new BackBlazeB2Config();
            config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
            config.Region = BackBlazeB2Region.UsWest001;

            BackBlazeB2Client client = new BackBlazeB2Client(config, driver);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-west-001.backblazeb2.com/GetObjectAsync", driver.SendResource);
        }
    }
}
#endif