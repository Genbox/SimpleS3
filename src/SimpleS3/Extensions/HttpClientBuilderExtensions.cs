using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Retry;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Genbox.SimpleS3.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public delegate TimeSpan BackoffTime(int retryAttempt);

        private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = (response) =>
        {
            // Polly default transient codes: >500 & 408
            // https://github.com/App-vNext/Polly.Extensions.Http/blob/master/src/Polly.Extensions.Http/HttpPolicyExtensions.cs
            return (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout;
        };

        public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            return builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { Proxy = proxy });
        }

        /// <summary>
        /// Adds a retry policy with 3 retries. Also adds a timeout policy that waits for 10 minutes before it terminates a request.
        /// </summary>
        public static IHttpClientBuilder AddDefaultHttpPolicy(this IHttpClientBuilder builder)
        {
            return builder.AddRetryPolicy(3).AddTimeoutPolicy(TimeSpan.FromMinutes(10));
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
            // Add a policy that will handle transient HTTP & Networking errors
            RetryPolicy<HttpResponseMessage> exceptionPolicy = Policy<HttpResponseMessage>
                // Handle network errors
                .Handle<IOException>()
                // Handle other HttpClient errors
                .Or<HttpRequestException>()
                // Handle Polly timeouts
                .Or<TimeoutRejectedException>()
                // Handle transient-error status codes
                .OrResult(TransientHttpStatusCodePredicate)
                // Action
                .WaitAndRetryAsync(retries, retryAttempt => backoffTime(retryAttempt));

            builder.AddPolicyHandler(exceptionPolicy);
            builder.Services.AddSingleton<IRequestStreamWrapper, RetryableBufferingStreamWrapper>();
            return builder;
        }

        public static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder builder, TimeSpan timeout)
        {
            TimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);
            builder.AddPolicyHandler(timeoutPolicy);
            builder.Services.AddSingleton<IRequestStreamWrapper, RetryableBufferingStreamWrapper>();
            return builder;
        }
    }
}