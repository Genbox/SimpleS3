using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.GenericS3;
using Genbox.SimpleS3.GenericS3.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GenericS3.Tests;

[SuppressMessage("Maintainability", "CA1515", Justification = "xUnit requires public classes to discover inherited tests.")]
public class StaticCreatorTests : ProviderClientTestBase<GenericS3Client>
{
    protected override string ExpectedObjectUrl => "https://myendpoint.com/GetObjectAsync";

    protected override GenericS3Client CreateClient(NullNetworkDriver driver) => new GenericS3Client(new GenericS3Config(Credentials(), "https://myendpoint.com", "us-east-1"), driver);

    protected override GenericS3Client CreateClient(NetworkConfig? networkConfig) => new GenericS3Client(Credentials(), "https://myendpoint.com", "us-east-1", networkConfig);

    protected override void ConfigureServices(IServiceCollection services) => services.AddGenericS3(config =>
    {
        config.Credentials = Credentials();
        config.Endpoint = "https://myendpoint.com";
        config.RegionCode = "us-east-1";
    });

    private static StringAccessKey Credentials() => new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
}