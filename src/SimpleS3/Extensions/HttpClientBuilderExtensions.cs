using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Genbox.SimpleS3.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            return builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { Proxy = proxy });
        }

        public static IHttpClientBuilder AddDefaultRetryPolicy(this IHttpClientBuilder builder)
        {
            Random random = new Random();

            // Policy is:
            // Retries: 3
            // Timeout: 2^attempt seconds (2, 4, 8 seconds) + -100 to 100 ms jitter
            builder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(random.Next(-100, 100))));

            return builder;
        }
    }
}