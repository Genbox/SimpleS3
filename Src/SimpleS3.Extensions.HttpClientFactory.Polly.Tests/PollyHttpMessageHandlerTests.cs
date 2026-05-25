using System.Diagnostics;
using System.Net;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Tests;

public class PollyHttpMessageHandlerTests
{
    [Fact]
    public void HandlerDoesNotStoreSharedPollyContext()
    {
        FieldInfo[] fields = typeof(PollyHttpMessageHandler).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.DoesNotContain(fields, field => field.FieldType == typeof(Context));
    }

    [Fact]
    public void UseRetryAndTimeoutAppliesNamedConfigToRetryableBufferingStreamWrapper()
    {
        ServiceCollection services = new ServiceCollection();
        TestHttpClientBuilder builder = new TestHttpClientBuilder(services, "custom");

        builder.UseRetryAndTimeout(config => config.MaxMemoryBufferSize = 123);

        using ServiceProvider provider = services.BuildServiceProvider();
        RetryableBufferingStreamWrapper wrapper = Assert.IsType<RetryableBufferingStreamWrapper>(provider.GetRequiredService<IRequestStreamWrapper>());
        FieldInfo configField = typeof(RetryableBufferingStreamWrapper).GetField("_config", BindingFlags.Instance | BindingFlags.NonPublic)!;
        PollyConfig config = Assert.IsType<PollyConfig>(configField.GetValue(wrapper));

        Assert.Equal(123, config.MaxMemoryBufferSize);
    }

    [Fact]
    public async Task UseRetryAndTimeoutHonorsNoDelayRetryMode()
    {
        ServiceCollection services = new ServiceCollection();
        using CountingHandler handler = new CountingHandler();

        services.AddHttpClient("custom")
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .UseRetryAndTimeout(config =>
                {
                    config.Retries = 1;
                    config.RetryMode = RetryMode.NoDelay;
                    config.Timeout = TimeSpan.FromSeconds(30);
                });

        await using ServiceProvider provider = services.BuildServiceProvider();
        HttpClient client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("custom");

        Stopwatch stopwatch = Stopwatch.StartNew();
        using HttpResponseMessage response = await client.GetAsync("https://example.com");
        stopwatch.Stop();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(2, handler.Calls);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(1), $"Retry delay took {stopwatch.Elapsed}.");
    }

    private sealed class TestHttpClientBuilder(IServiceCollection services, string name) : IHttpClientBuilder
    {
        public IServiceCollection Services { get; } = services;
        public string Name { get; } = name;
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }
    }
}