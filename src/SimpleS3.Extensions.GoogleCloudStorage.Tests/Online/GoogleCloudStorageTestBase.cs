using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage.Tests.Online
{
    public abstract class GoogleCloudStorageTestBase : OnlineTestBase
    {
        protected GoogleCloudStorageTestBase(ITestOutputHelper outputHelper) : base(outputHelper, "TestSetup-GoogleCloudStorage") { }

        protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
        {
            //Call the base method here. we need to overwrite the config with the GoogleCloudStorage profile
            base.ConfigureCoreBuilder(coreBuilder, configuration);

            coreBuilder.UseGoogleCloudStorage();
        }
    }
}