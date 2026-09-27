using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.AmazonS3.Extensions;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.ProviderBase;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.AmazonS3.Tests;

[SuppressMessage("Maintainability", "CA1515", Justification = "xUnit requires public classes to discover inherited tests.")]
public class StaticCreatorTests : ProviderClientTestBase<AmazonS3Client>
{
    protected override string ExpectedObjectUrl => "https://testbucket.s3.us-east-1.amazonaws.com/GetObjectAsync";

    protected override AmazonS3Client CreateClient(NullNetworkDriver driver) => new AmazonS3Client(new AmazonS3Config(Credentials(), AmazonS3Region.UsEast1), driver);

    protected override AmazonS3Client CreateClient(NetworkConfig? networkConfig) => new AmazonS3Client(Credentials(), AmazonS3Region.UsEast1, networkConfig);

    protected override void ConfigureServices(IServiceCollection services) => services.AddAmazonS3(config =>
    {
        config.Credentials = Credentials();
        config.Region = AmazonS3Region.UsEast1;
    });

    private static StringAccessKey Credentials() => new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
}