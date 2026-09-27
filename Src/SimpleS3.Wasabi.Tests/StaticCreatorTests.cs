using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.Wasabi;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.Wasabi.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Wasabi.Tests;

public class StaticCreatorTests : ProviderClientTestBase<WasabiClient>
{
    protected override string ExpectedObjectUrl => "https://testbucket.s3.eu-central-1.wasabisys.com/GetObjectAsync";

    protected override WasabiClient CreateClient(NullNetworkDriver driver) => new WasabiClient(new WasabiConfig(Credentials(), WasabiRegion.EuCentral1), driver);

    protected override WasabiClient CreateClient(NetworkConfig? networkConfig) => new WasabiClient(Credentials(), WasabiRegion.EuCentral1, networkConfig);

    protected override void ConfigureServices(IServiceCollection services) => services.AddWasabi(config =>
    {
        config.Credentials = Credentials();
        config.Region = WasabiRegion.EuCentral1;
    });

    private static StringAccessKey Credentials() => new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
}