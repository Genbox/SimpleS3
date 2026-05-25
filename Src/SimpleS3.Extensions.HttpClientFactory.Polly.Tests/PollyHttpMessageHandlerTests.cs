using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
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

    private sealed class TestHttpClientBuilder(IServiceCollection services, string name) : IHttpClientBuilder
    {
        public IServiceCollection Services { get; } = services;
        public string Name { get; } = name;
    }
}