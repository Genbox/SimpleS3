using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.TestBase.Code;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ProviderTests
{
    private static async Task<string?> TestProvider(Action<SimpleS3Config> configure)
    {
        ServiceCollection service = new ServiceCollection();

        SimpleS3CoreServices.AddSimpleS3Core(service, x =>
        {
            x.RegionCode = "myregion";
            x.Credentials = new StringAccessKey("key", "secret");
            configure(x);
        });

        service.AddSingleton<INetworkDriver, NullNetworkDriver>(); //A dummy network driver

        await using ServiceProvider serviceCollection = service.BuildServiceProvider();
        IObjectClient? objectClient = serviceCollection.GetService<IObjectClient>();
        Assert.NotNull(objectClient);

        NullNetworkDriver driver = (NullNetworkDriver)serviceCollection.GetRequiredService<INetworkDriver>();
        await objectClient.GetObjectAsync("bucket", "key");
        return driver.LastUrl;
    }

    [Fact]
    internal async Task CustomProviderEndpoint()
    {
        string? url = await TestProvider(config =>
        {
            config.Endpoint = "http://doesnotexist.local";
            config.NamingMode = NamingMode.PathStyle;
        });

        Assert.Equal("http://doesnotexist.local/bucket/key", url);
    }

    [Fact]
    internal async Task CustomProviderEndpointTemplateVirtualHost()
    {
        string? url = await TestProvider(config =>
        {
            config.Endpoint = "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com";
            config.NamingMode = NamingMode.VirtualHost;
        });

        Assert.Equal("https://bucket.s3.myregion.amazonaws.com/key", url);
    }

    [Fact]
    internal async Task CustomProviderEndpointTemplatePathStyle()
    {
        string? url = await TestProvider(config =>
        {
            config.Endpoint = "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com";
            config.NamingMode = NamingMode.PathStyle;
        });

        Assert.Equal("https://s3.myregion.amazonaws.com/bucket/key", url);
    }
}