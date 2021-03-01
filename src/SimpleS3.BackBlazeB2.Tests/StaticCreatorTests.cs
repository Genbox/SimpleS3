using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Extensions;
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

            B2Config config = new B2Config();
            config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
            config.Region = B2Region.UsWest001;

            B2Client client = new B2Client(config, driver);

            await client.PutObjectStringAsync("testbucket", "PutAsync", "data").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-west-001.backblazeb2.com/PutAsync", driver.SendResource);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-west-001.backblazeb2.com/GetObjectAsync", driver.SendResource);

            Assert.True((await client.GetObjectAsync("testbucket", "GetDataAsync").ConfigureAwait(false)).IsSuccess);
            Assert.Equal("https://testbucket.s3.us-west-001.backblazeb2.com/GetDataAsync", driver.SendResource);
        }
    }
}