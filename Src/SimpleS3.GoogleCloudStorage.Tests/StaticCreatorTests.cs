using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;

namespace Genbox.SimpleS3.GoogleCloudStorage.Tests;

public class StaticCreatorTests
{
    [Fact]
    public async Task StaticClient()
    {
        NullNetworkDriver driver = new NullNetworkDriver();

        GoogleCloudStorageConfig config = new GoogleCloudStorageConfig("GOOGTS7C7FUP3AIRVJTE2BCDKINBTES3HC2GY5CBFJDCQ2SYHV6A6XXVTJFSA", "bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ", GoogleCloudStorageRegion.EuropeWest1);
        using GoogleCloudStorageClient client = new GoogleCloudStorageClient(config, driver);

        await client.GetObjectAsync("testbucket", "GetObjectAsync");
        Assert.Equal("https://testbucket.storage.googleapis.com/GetObjectAsync", driver.LastUrl);
    }
}