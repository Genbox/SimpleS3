using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.TestBase
{
    public abstract class OfflineTestBase : UnitTestBase
    {
        protected OfflineTestBase(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            BucketName = "main-test-bucket";
        }

        protected override void ConfigureConfig(Config config)
        {
            config.RegionCode = "eu-west-1";
            config.Credentials = new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");

            base.ConfigureConfig(config);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            //Call TryAdd here to ensure that drivers added in the ConfigureCoreBuilder are respected
            services.TryAddSingleton<INetworkDriver, NullNetworkDriver>();

            base.ConfigureServices(services);
        }
    }
}