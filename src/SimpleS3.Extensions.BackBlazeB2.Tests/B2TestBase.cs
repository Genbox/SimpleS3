using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.TestBase;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Tests
{
    public abstract class B2TestBase : OnlineTestBase
    {
        protected B2TestBase(ITestOutputHelper outputHelper) : base(outputHelper) { }

        protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
        {
            coreBuilder.UseBackBlazeB2();

            string profileName = "TestSetup-BackBlazeB2";

            coreBuilder.UseProfileManager()
                       .BindConfigToProfile(profileName)
                       .UseDataProtection();
        }
    }
}