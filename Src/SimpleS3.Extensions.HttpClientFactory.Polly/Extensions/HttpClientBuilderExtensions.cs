using System.Net;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;

public static class HttpClientBuilderExtensions
{
    private static readonly Func<HttpResponseMessage, bool> _statusCodes = resp => (int)resp.StatusCode >= (int)HttpStatusCode.InternalServerError || resp.StatusCode == HttpStatusCode.RequestTimeout;
    private static readonly Random _rng = new Random();

    public static IHttpClientBuilder UseRetryAndTimeout(this IHttpClientBuilder builder, int retries, TimeSpan timeout)
    {
        builder.Services.Configure<PollyConfig>(x =>
        {
            x.Timeout = timeout;
            x.Retries = retries;
        });

        return UseRetryAndTimeout(builder);
    }

    public static IHttpClientBuilder UseRetryAndTimeout(this IHttpClientBuilder builder, Action<PollyConfig> configure)
    {
        builder.Services.Configure(configure);
        return UseRetryAndTimeout(builder);
    }

    public static IHttpClientBuilder UseRetryAndTimeout(this IHttpClientBuilder builder)
    {
        //Add IRequestStreamWrapper if it does not already exist. We need to add it such that it is combined with other IRequestStreamWrapper
        if (builder.Services.TryAddEnumerableRet(ServiceDescriptor.Singleton<IRequestStreamWrapper, RetryableBufferingStreamWrapper>()))
        {
            builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, (x, provider) =>
            {
                IOptions<PollyConfig> options = provider.GetRequiredService<IOptions<PollyConfig>>();
                PollyConfig config = options.Value;

                // Add a policy that will handle transient HTTP & Networking errors
                PolicyBuilder<HttpResponseMessage> b = Policy<HttpResponseMessage>
                                                       .Handle<IOException>() // Handle network errors
                                                       .Or<HttpRequestException>() // Handle other HttpClient errors
                                                       .Or<TimeoutRejectedException>() // Handle Polly timeouts
                                                       .OrResult(_statusCodes); // Handle transient-error status codes

                //When we use Polly, we don't want to use HttpClient's weird timeout
                x.HttpClientActions.Add(client => client.Timeout = TimeSpan.FromHours(100));

                AsyncRetryPolicy<HttpResponseMessage> retryPolicy = b.WaitAndRetryAsync(config.Retries, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                    TimeSpan.FromMilliseconds(_rng.Next(0, (int)config.MaxRandomDelay.TotalMilliseconds)));

                AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(config.Timeout);

                IAsyncPolicy<HttpResponseMessage>? finalPolicy = retryPolicy.WrapAsync(timeoutPolicy);
                x.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(new PollyHttpMessageHandler(finalPolicy)));
            });
        }

        return builder;
    }
}