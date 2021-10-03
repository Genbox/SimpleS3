#if COMMERCIAL
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;
using Xunit;

namespace Genbox.SimpleS3.GoogleCloudStorage.Tests
{
    public class StaticCreatorTests
    {
        [Fact]
        public async Task StaticClient()
        {
            FakeNetworkDriver driver = new FakeNetworkDriver();

            GoogleCloudStorageConfig config = new GoogleCloudStorageConfig();
            config.Credentials = new StringAccessKey("GOOGTS7C7FUP3AIRVJTE2BCDKINBTES3HC2GY5CBFJDCQ2SYHV6A6XXVTJFSA", "bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ");
            config.Region = GoogleCloudStorageRegion.EuropeWest1;

            GoogleCloudStorageClient client = new GoogleCloudStorageClient(config, driver);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("https://testbucket.storage.googleapis.com/GetObjectAsync", driver.SendResource);
        }
    }
}
#endif