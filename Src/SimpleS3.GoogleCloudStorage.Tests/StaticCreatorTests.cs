using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;
using Genbox.SimpleS3.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GoogleCloudStorage.Tests;

public class StaticCreatorTests : ProviderClientTestBase<GoogleCloudStorageClient>
{
    protected override string ExpectedObjectUrl => "https://testbucket.storage.googleapis.com/GetObjectAsync";

    protected override GoogleCloudStorageClient CreateClient(NullNetworkDriver driver) => new GoogleCloudStorageClient(new GoogleCloudStorageConfig(Credentials(), GoogleCloudStorageRegion.EuropeWest1), driver);

    protected override GoogleCloudStorageClient CreateClient(NetworkConfig? networkConfig) => new GoogleCloudStorageClient(Credentials(), GoogleCloudStorageRegion.EuropeWest1, networkConfig);

    protected override void ConfigureServices(IServiceCollection services) => services.AddGoogleCloudStorage(config =>
    {
        config.Credentials = Credentials();
        config.Region = GoogleCloudStorageRegion.EuropeWest1;
    });

    private static StringAccessKey Credentials() => new StringAccessKey("GOOGTS7C7FUP3AIRVJTE2BCDKINBTES3HC2GY5CBFJDCQ2SYHV6A6XXVTJFSA", "bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ");
}