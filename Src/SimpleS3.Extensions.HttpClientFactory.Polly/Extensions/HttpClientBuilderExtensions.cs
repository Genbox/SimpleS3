using System.Net;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;

[PublicAPI]
public static class HttpClientBuilderExtensions
{
    private static readonly Func<HttpResponseMessage, bool> _statusCodes = resp => (int)resp.StatusCode >= (int)HttpStatusCode.InternalServerError || resp.StatusCode == HttpStatusCode.RequestTimeout;
    private static readonly Random _rng = new Random();

    public static IHttpClientBuilder UseRetryAndTimeout(this IHttpClientBuilder builder, int retries, TimeSpan timeout)
    {
        builder.Services.Configure<PollyConfig>(builder.Name, x =>
        {
            x.Timeout = timeout;
            x.Retries = retries;
        });

        return UseRetryAndTimeout(builder);
    }

    public static IHttpClientBuilder UseRetryAndTimeout(this IHttpClientBuilder builder, Action<PollyConfig> configure)
    {
        builder.Services.Configure(builder.Name, configure);
        return UseRetryAndTimeout(builder);
    }

    public static IHttpClientBuilder UseRetryAndTimeout(this IHttpClientBuilder builder)
    {
        //Add IRequestStreamWrapper if it does not already exist. We need to add it such that it is combined with other IRequestStreamWrapper
        builder.Services.AddKeyedSingleton<IRequestStreamWrapper>(builder.Name, (provider, _) => ActivatorUtilities.CreateInstance<RetryableBufferingStreamWrapper>(provider, builder.Name));

        if (builder.Name == ServiceBuilderBase.DefaultName)
            builder.Services.AddSingleton<IRequestStreamWrapper>(provider => ActivatorUtilities.CreateInstance<RetryableBufferingStreamWrapper>(provider, builder.Name));

        if (TryAddPolicyRegistration(builder.Services, builder.Name))
        {
            builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, (x, provider) =>
            {
                IOptionsMonitor<PollyConfig> options = provider.GetRequiredService<IOptionsMonitor<PollyConfig>>();
                PollyConfig config = options.Get(builder.Name);

                // Add a policy that will handle transient HTTP & Networking errors
                PolicyBuilder<HttpResponseMessage> b = Policy<HttpResponseMessage>
                                                       .Handle<IOException>() // Handle network errors
                                                       .Or<HttpRequestException>() // Handle other HttpClient errors
                                                       .Or<TimeoutRejectedException>() // Handle Polly timeouts
                                                       .OrResult(_statusCodes); // Handle transient-error status codes

                //When we use Polly, we don't want to use HttpClient's weird timeout
                x.HttpClientActions.Add(client => client.Timeout = TimeSpan.FromHours(100));

                AsyncRetryPolicy<HttpResponseMessage> retryPolicy = b.WaitAndRetryAsync(config.Retries, retryAttempt => GetDelay(config, retryAttempt));

                AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(config.Timeout);

                IAsyncPolicy<HttpResponseMessage>? finalPolicy = retryPolicy.WrapAsync(timeoutPolicy);
                x.HttpMessageHandlerBuilderActions.Add(h => h.AdditionalHandlers.Add(new PollyHttpMessageHandler(finalPolicy)));
            });
        }

        return builder;
    }

    private static bool TryAddPolicyRegistration(IServiceCollection services, string name)
    {
        foreach (ServiceDescriptor descriptor in services)
        {
            if (descriptor.ServiceType == typeof(PollyPolicyRegistration) && descriptor.ImplementationInstance is PollyPolicyRegistration registration && registration.Name == name)
                return false;
        }

        services.AddSingleton(new PollyPolicyRegistration(name));
        return true;
    }

    private static TimeSpan GetDelay(PollyConfig config, int retryAttempt) => config.RetryMode switch
    {
        RetryMode.NoDelay => TimeSpan.Zero,
        RetryMode.LinearDelay => TimeSpan.FromSeconds(retryAttempt),
        RetryMode.LinearDelayJitter => TimeSpan.FromSeconds(retryAttempt) + GetJitter(config),
        RetryMode.ExponentialBackoff => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        RetryMode.ExponentialBackoffJitter => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + GetJitter(config),
        _ => throw new InvalidOperationException($"Unsupported retry mode: {config.RetryMode}")
    };

    private static TimeSpan GetJitter(PollyConfig config)
    {
        int maxDelay = (int)config.MaxRandomDelay.TotalMilliseconds;

        lock (_rng)
            return TimeSpan.FromMilliseconds(_rng.Next(0, maxDelay));
    }

    private sealed class PollyPolicyRegistration(string name)
    {
        public string Name { get; } = name;
    }
}