using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.AmazonS3.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Advanced;

public static class NetworkRetryAndTimeout
{
    /// <summary>
    /// Example on how to setup a S3 client with retry and timeout using dependency injection
    /// </summary>
    private static void ExampleWithDependencyInjection()
    {
        int retries = 3;
        TimeSpan timeout = TimeSpan.FromMinutes(5);

        ServiceCollection services = new ServiceCollection();

        IClientBuilder builder = services.AddAmazonS3(config =>
        {
            config.Credentials = new StringAccessKey("YourKeyId", "YourAccessKey");
            config.Region = AmazonS3Region.EuWest1;
        });

        //Here we supply the retry and timeout. Note that it can be supplied different ways using this method.
        builder.HttpBuilder.UseRetryAndTimeout(retries, timeout);

        using ServiceProvider provider = services.BuildServiceProvider();
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
    }

    /// <summary>
    /// Example on how to setup a S3 client with retry and timeout
    /// </summary>
    private static void ExampleWithClient()
    {
        NetworkConfig netConf = new NetworkConfig();
        netConf.Retries = 3;
        netConf.Timeout = TimeSpan.FromMinutes(5);

        using AmazonS3Client client = new AmazonS3Client("YourKeyId", "YourAccessKey", AmazonS3Region.EuWest1, netConf);
    }
}