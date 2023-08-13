using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.TestBase.Code;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Core.TestBase;

public abstract class OfflineTestBase : UnitTestBase
{
    protected OfflineTestBase(ITestOutputHelper outputHelper) : base(outputHelper, "main-test-bucket") {}

    protected override void ConfigureConfig(SimpleS3Config config)
    {
        config.RegionCode = "eu-west-1";
        config.Credentials = new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");
        config.EndpointTemplate = "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com";

        base.ConfigureConfig(config);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        //Call TryAdd here to ensure that drivers added in the ConfigureCoreBuilder are respected
        services.TryAddSingleton<INetworkDriver, NullNetworkDriver>();

        base.ConfigureServices(services);
    }
}