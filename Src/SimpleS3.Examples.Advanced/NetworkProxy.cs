using System.Net;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.AmazonS3.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Advanced;

public static class NetworkProxy
{
    /// <summary>
    /// Example on how to setup a S3 client with proxy using dependency injection
    /// </summary>
    private static void ExampleWithDependencyInjection()
    {
        ServiceCollection services = new ServiceCollection();

        IClientBuilder builder = services.AddAmazonS3(config =>
        {
            config.Credentials = new StringAccessKey("YourKeyId", "YourAccessKey");
            config.Region = AmazonS3Region.EuWest1;
        });

        //Here we supply the proxy
        builder.HttpBuilder.UseProxy(new WebProxy("http://127.0.0.1:8888"));

        using ServiceProvider provider = services.BuildServiceProvider();
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
    }

    /// <summary>
    /// Example on how to setup a S3 client with proxy
    /// </summary>
    private static void ExampleWithClient()
    {
        NetworkConfig netConf = new NetworkConfig();
        netConf.Proxy = new WebProxy("http://127.0.0.1:8888");

        using AmazonS3Client client = new AmazonS3Client("YourKeyId", "YourAccessKey", AmazonS3Region.EuWest1, netConf);
    }
}