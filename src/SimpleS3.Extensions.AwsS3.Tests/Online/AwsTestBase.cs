using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.AwsS3.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online
{
    public abstract class AwsTestBase : OnlineTestBase
    {
        protected AwsTestBase(ITestOutputHelper outputHelper) : base(outputHelper) { }

        protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
        {
            //Call the base method here. we need to overwrite the config with the BackBlazeB2 profile
            base.ConfigureCoreBuilder(coreBuilder, configuration);

            coreBuilder.UseAwsS3();

            string profileName = "TestSetup-AmazonS3";

            coreBuilder.UseProfileManager()
                       .BindConfigToProfile(profileName)
                       .UseDataProtection();
        }
    }
}