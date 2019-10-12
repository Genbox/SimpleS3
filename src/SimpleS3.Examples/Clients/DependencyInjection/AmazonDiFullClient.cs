using System;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Genbox.SimpleS3.Examples.Clients.DependencyInjection
{
    /// <summary>This is an example that shows the full capabilities of SimpleS3.</summary>
    public static class AmazonDiFullClient
    {
        public static S3Client Create()
        {
            //In this example we are using Dependency Injection (DI) using Microsoft's DI framework
            ServiceCollection services = new ServiceCollection();

            //We use Microsoft.Extensions.Configuration framework here to load our config file
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("Config.json", false);
            IConfigurationRoot root = configBuilder.Build();

            //We use Microsoft.Extensions.Logging here to add support for logging
            services.AddLogging(x =>
            {
                x.AddConsole();
                x.AddConfiguration(root.GetSection("Logging"));
            });

            //Here we setup our S3Client
            IS3ClientBuilder clientBuilder = services.AddSimpleS3Core(s3Config =>
            {
                root.Bind(s3Config);

                s3Config.Credentials = new StringAccessKey("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");
                s3Config.Region = AwsRegion.UsEast1;
                s3Config.Endpoint = new Uri("https://play.min.io:9000/");
            });

            //We enable HTTP Factory support here.
            IHttpClientBuilder httpBuilder = clientBuilder.UseHttpClientFactory();

            //Every 5 minutes we create a new connection, thereby reacting to DNS changes
            httpBuilder.SetHandlerLifetime(TimeSpan.FromMinutes(5));

            //Uncomment this line if you want to use a proxy
            //httpBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { Proxy = new WebProxy("http://<YourProxy>:<ProxyPort>") });

            //Here we enable retrying. We retry 3 times with a delay of 600 ms between attempts. For more examples, see https://github.com/App-vNext/Polly
            httpBuilder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //Finally we build the service provider and return the S3Client
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}