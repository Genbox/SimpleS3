using System.Net;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;

public static class HttpClientBuilderExtensions
{
    public delegate TimeSpan BackoffTime(int retryAttempt);

    // Polly default transient codes: >500 & 408
    // https://github.com/App-vNext/Polly.Extensions.Http/blob/master/src/Polly.Extensions.Http/HttpPolicyExtensions.cs
    private static readonly Func<HttpResponseMessage, bool> _transientHttpStatusCodePredicate = resp => (int)resp.StatusCode >= 500 || resp.StatusCode == HttpStatusCode.RequestTimeout;

    /// <summary>Adds a retry policy with 3 retries. Also adds a timeout policy that waits for 10 minutes before it terminates
    /// a request.</summary>
    public static IHttpClientBuilder UseDefaultHttpPolicy(this IHttpClientBuilder builder) => builder.UseRetryPolicy(3).UseTimeoutPolicy(TimeSpan.FromMinutes(10));

    public static IHttpClientBuilder UseRetryPolicy(this IHttpClientBuilder builder, int retries)
    {
        Random random = new Random();

        // Policy is:
        // Retries: 3
        // Timeout: 2^attempt seconds (2, 4, 8 seconds) + -100 to 100 ms jitter
        return builder.UseRetryPolicy(retries, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
            TimeSpan.FromMilliseconds(random.Next(-100, 100)));
    }

    public static IHttpClientBuilder UseRetryPolicy(this IHttpClientBuilder builder, int retries, BackoffTime backoffTime)
    {
        // Add a policy that will handle transient HTTP & Networking errors
        AsyncRetryPolicy<HttpResponseMessage> exceptionPolicy = Policy<HttpResponseMessage>

                                                                // Handle network errors
                                                                .Handle<IOException>()

                                                                // Handle other HttpClient errors
                                                                .Or<HttpRequestException>()

                                                                // Handle Polly timeouts
                                                                .Or<TimeoutRejectedException>()

                                                                // Handle transient-error status codes
                                                                .OrResult(_transientHttpStatusCodePredicate)

                                                                // Action
                                                                .WaitAndRetryAsync(retries, retryAttempt => backoffTime(retryAttempt));

        builder.AddPolicyHandler(exceptionPolicy);
        builder.Services.AddSingleton<IRequestStreamWrapper, RetryableBufferingStreamWrapper>();
        return builder;
    }

    public static IHttpClientBuilder UseTimeoutPolicy(this IHttpClientBuilder builder, TimeSpan timeout)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);
        builder.AddPolicyHandler(timeoutPolicy);
        return builder;
    }
}