using System;
using System.Net;
using System.Net.Http;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Retry;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Genbox.SimpleS3.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public delegate TimeSpan BackoffTime(int retryAttempt);

        public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            return builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { Proxy = proxy });
        }

        public static IHttpClientBuilder AddDefaultRetryPolicy(this IHttpClientBuilder builder)
        {
            return builder.AddRetryPolicy(3);
        }

        public static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder builder, int retries)
        {
            Random random = new Random();

            // Policy is:
            // Retries: 3
            // Timeout: 2^attempt seconds (2, 4, 8 seconds) + -100 to 100 ms jitter
            return builder.AddRetryPolicy(retries, retryAttempt =>
                                                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                                    TimeSpan.FromMilliseconds(random.Next(-100, 100)));
        }

        public static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder builder, int retries, BackoffTime backoffTime)
        {
            builder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(retries, retryAttempt => backoffTime(retryAttempt)));

            // TODO: Add this first, before chunked streamer?
            builder.Services.AddSingleton<IRequestStreamWrapper, RetryableBufferingStreamWrapper>();

            return builder;
        }
    }
}