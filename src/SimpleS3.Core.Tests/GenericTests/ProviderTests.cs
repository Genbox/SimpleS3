using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.TestBase.Code;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests
{
    public class ProviderTests
    {
        /// <summary>
        /// This test checks if users can depend on just SimpleS3.Core + a network driver (no provider such as Amazon, Google, BackBlaze etc.) and still use the library with Region and Endpoint.
        /// </summary>
        [Fact]
        internal void CanRunWithoutProvider()
        {
            ServiceCollection service = new ServiceCollection();
            SimpleS3CoreServices.AddSimpleS3Core(service);
            service.AddSingleton<INetworkDriver, NullNetworkDriver>(); //A dummy network driver
            service.Configure<Config>(x =>
            {
                x.RegionCode = "myregion";
                x.Credentials = new StringAccessKey("key", "secret");
            });

            using ServiceProvider serviceCollection = service.BuildServiceProvider();
            IObjectClient? objectClient = serviceCollection.GetService<IObjectClient>();
            Assert.NotNull(objectClient);
        }
    }
}