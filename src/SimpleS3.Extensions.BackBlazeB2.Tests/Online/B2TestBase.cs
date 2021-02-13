using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Tests.Online
{
    public abstract class B2TestBase : OnlineTestBase
    {
        protected B2TestBase(ITestOutputHelper outputHelper) : base(outputHelper, "TestSetup-BackBlazeB2") { }

        protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
        {
            //Call the base method here. we need to overwrite the config with the BackBlazeB2 profile
            base.ConfigureCoreBuilder(coreBuilder, configuration);

            coreBuilder.UseBackBlazeB2();
        }
    }
}