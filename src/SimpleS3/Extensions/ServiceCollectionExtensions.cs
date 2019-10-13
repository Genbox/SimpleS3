using System;
using System.Net;
using System.Net.Http;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Genbox.SimpleS3.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config, IServiceProvider> configureS3)
        {
            collection?.Configure(configureS3);
            IS3ClientBuilder builder = collection.AddSimpleS3Core();
            builder.UseS3Client();

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();

            httpBuilder.SetHandlerLifetime(TimeSpan.FromMinutes(5));

            Random random = new Random();

            // Policy is:
            // Retries: 3
            // Timeout: 2^attempt seconds (2, 4, 8 seconds) + -100 to 100 ms jitter
            httpBuilder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(random.Next(-100, 100))));

            return httpBuilder;
        }

        public static IHttpClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3)
        {
            return collection.AddSimpleS3((config, provider) => configureS3(config));
        }

        public static IHttpClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3, HttpMessageHandler handler)
        {
            IHttpClientBuilder builder = collection.AddSimpleS3((config, provider) => configureS3(config));

            if (handler != null)
                builder.ConfigurePrimaryHttpMessageHandler(() => handler);

            return builder;
        }

        public static IHttpClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3, IWebProxy proxy)
        {
            return collection.AddSimpleS3(configureS3, new HttpClientHandler {Proxy = proxy});
        }
    }
}