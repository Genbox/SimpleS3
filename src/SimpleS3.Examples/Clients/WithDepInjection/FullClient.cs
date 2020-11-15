using System;
using System.Net;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Aws;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IHttpClientBuilder = Genbox.SimpleS3.Extensions.HttpClient.IHttpClientBuilder;

namespace Genbox.SimpleS3.Examples.Clients.WithDepInjection
{
    /// <summary>This is an example that shows the full capabilities of SimpleS3.</summary>
    public static class FullClient
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region, IWebProxy? proxy = null)
        {
            //In this example we are using using Microsoft's Dependency Injection (DI) framework
            ServiceCollection services = new ServiceCollection();

            //We use Microsoft.Extensions.Logging here to add support for logging
            services.AddLogging(x =>
            {
                x.AddConsole();
            });

            //Here we create a core client without a network driver
            ICoreBuilder coreBuilder = services.AddSimpleS3Core(s3Config =>
            {
                s3Config.Credentials = new StringAccessKey(keyId, accessKey);
                s3Config.Region = region;

                //Note that you can also override other configuration values here, even if they were bound to something else above. The values here take precedence.
            });

            //The default client is HttpClientFactory, but to show how we can change this, we use HttpClient here.
            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClient();

            if (proxy != null)
                httpBuilder.WithProxy(proxy);

            //This adds the S3Client service. This service combines ObjectClient, MultipartClient and BucketClient into a single client. Makes it easier for new people to use the library.
            coreBuilder.UseS3Client();

            //Finally we build the service provider and return the S3Client
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}