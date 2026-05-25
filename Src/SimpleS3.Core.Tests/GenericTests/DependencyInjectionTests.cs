using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using SimpleS3HttpVersion = Genbox.SimpleS3.Core.Common.HttpVersion;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class DependencyInjectionTests
{
    [Fact]
    public void UseHttpClientFactoryAppliesNamedHttpBuilderActions()
    {
        ServiceCollection services = new ServiceCollection();
        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

        builder.UseHttpClientFactory();
        services.Configure<HttpBuilderActions>(builder.Name, actions => actions.HttpClientActions.Add((_, client) => client.DefaultRequestHeaders.Add("x-test-action", "applied")));

        using ServiceProvider provider = services.BuildServiceProvider();
        IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();

        using HttpClient client = factory.CreateClient(builder.Name);

        Assert.True(client.DefaultRequestHeaders.Contains("x-test-action"));
        Assert.False(client.DefaultRequestHeaders.TransferEncodingChunked);
    }

    [Fact]
    public void UseHttpClientFactoryAppliesNamedConfig()
    {
        ServiceCollection services = new ServiceCollection();
        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services, name: "custom");

        builder.UseHttpClientFactory(config => config.HttpVersion = SimpleS3HttpVersion.Http2);

        using ServiceProvider provider = services.BuildServiceProvider();
        INetworkDriver driver = provider.GetRequiredService<INetworkDriver>();
        FieldInfo configField = typeof(HttpClientFactoryNetworkDriver).GetField("_config", BindingFlags.Instance | BindingFlags.NonPublic)!;
        HttpClientFactoryConfig config = Assert.IsType<HttpClientFactoryConfig>(configField.GetValue(driver));

        Assert.Equal(SimpleS3HttpVersion.Http2, config.HttpVersion);
    }

    [Fact]
    public void UseRetryAndTimeoutRegistersPoliciesForEachNamedBuilder()
    {
        ServiceCollection services = new ServiceCollection();

        IHttpClientBuilder first = new TestHttpClientBuilder(services, "first");
        first.UseRetryAndTimeout();

        IHttpClientBuilder second = new TestHttpClientBuilder(services, "second");
        second.UseRetryAndTimeout();

        using ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<HttpClientFactoryOptions> options = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>();

        Assert.Single(options.Get(first.Name).HttpMessageHandlerBuilderActions);
        Assert.Single(options.Get(second.Name).HttpMessageHandlerBuilderActions);
    }

    private sealed class TestHttpClientBuilder(IServiceCollection services, string name) : IHttpClientBuilder
    {
        public IServiceCollection Services { get; } = services;
        public string Name { get; } = name;
    }
}