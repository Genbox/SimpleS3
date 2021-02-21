using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.AwsS3.Extensions;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online
{
    public abstract class AwsTestBase : OnlineTestBase
    {
        protected AwsTestBase(ITestOutputHelper outputHelper) : base(outputHelper, "TestSetup-AmazonS3") { }

        protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
        {
            coreBuilder.UseAwsS3();
            base.ConfigureCoreBuilder(coreBuilder, configuration);
        }
    }
}