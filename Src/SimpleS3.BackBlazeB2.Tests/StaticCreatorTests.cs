using Genbox.SimpleS3.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.BackBlazeB2;
using Genbox.SimpleS3.ProviderBase;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.BackBlazeB2.Tests;

public class StaticCreatorTests : ProviderClientTestBase<BackBlazeB2Client>
{
    protected override string ExpectedObjectUrl => "https://testbucket.s3.us-west-001.backblazeb2.com/GetObjectAsync";

    protected override BackBlazeB2Client CreateClient(NullNetworkDriver driver) => new BackBlazeB2Client(new BackBlazeB2Config(Credentials(), BackBlazeB2Region.UsWest001), driver);

    protected override BackBlazeB2Client CreateClient(NetworkConfig? networkConfig) => new BackBlazeB2Client(Credentials(), BackBlazeB2Region.UsWest001, networkConfig);

    protected override void ConfigureServices(IServiceCollection services) => services.AddBackBlazeB2(config =>
    {
        config.Credentials = Credentials();
        config.Region = BackBlazeB2Region.UsWest001;
    });

    private static StringAccessKey Credentials() => new StringAccessKey("61a951f6abb8", "0000000000000000000000000000000");
}