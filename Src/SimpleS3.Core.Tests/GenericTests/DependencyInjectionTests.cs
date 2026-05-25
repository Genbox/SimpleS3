using System.Net;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.GenericS3.Extensions;
using Genbox.SimpleS3.GenericS3;
using Genbox.SimpleS3.GenericS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using SimpleS3HttpVersion = Genbox.SimpleS3.Core.Common.HttpVersion;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class DependencyInjectionTests
{
    [Fact]
    public async Task AddGenericS3SupportsEndToEndNamedDependencyIsolation()
    {
        ServiceCollection services = new ServiceCollection();
        using RecordingHandler firstHandler = new RecordingHandler();
        using RecordingHandler secondHandler = new RecordingHandler();

        IClientBuilder firstBuilder = services.AddGenericS3(config =>
        {
            config.Endpoint = "https://first.example.com";
            config.RegionCode = "first-region";
            config.NamingMode = NamingMode.PathStyle;
            config.PayloadSignatureMode = SignatureMode.Unsigned;
        }, "first");
        firstBuilder.HttpBuilder.ConfigurePrimaryHttpMessageHandler(() => firstHandler);

        IClientBuilder secondBuilder = services.AddGenericS3(config =>
        {
            config.Endpoint = "https://second.example.com";
            config.RegionCode = "second-region";
            config.NamingMode = NamingMode.PathStyle;
            config.PayloadSignatureMode = SignatureMode.Unsigned;
        }, "second");
        secondBuilder.HttpBuilder.ConfigurePrimaryHttpMessageHandler(() => secondHandler);

        await using ServiceProvider provider = services.BuildServiceProvider();
        GenericS3Client firstClient = provider.GetRequiredKeyedService<GenericS3Client>("first");
        GenericS3Client secondClient = provider.GetRequiredKeyedService<GenericS3Client>("second");

        await firstClient.HeadBucketAsync("first-bucket", token: TestContext.Current.CancellationToken);
        await secondClient.HeadBucketAsync("second-bucket", token: TestContext.Current.CancellationToken);

        Uri firstRequest = Assert.Single(firstHandler.Requests);
        Uri secondRequest = Assert.Single(secondHandler.Requests);
        IOptionsMonitor<SimpleS3Config> options = provider.GetRequiredService<IOptionsMonitor<SimpleS3Config>>();

        Assert.Equal("first.example.com", firstRequest.Host);
        Assert.Equal("/first-bucket/", firstRequest.AbsolutePath);
        Assert.Equal("second.example.com", secondRequest.Host);
        Assert.Equal("/second-bucket/", secondRequest.AbsolutePath);
        Assert.Equal("https://first.example.com", options.Get("first").Endpoint);
        Assert.Equal("https://second.example.com", options.Get("second").Endpoint);
    }

    [Fact]
    public void AddGenericS3CustomNamesDoNotRegisterUnkeyedClientAliases()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddGenericS3(config =>
        {
            config.Endpoint = "https://first.example.com";
            config.RegionCode = "first-region";
            config.NamingMode = NamingMode.PathStyle;
        }, "first");

        services.AddGenericS3(config =>
        {
            config.Endpoint = "https://second.example.com";
            config.RegionCode = "second-region";
            config.NamingMode = NamingMode.PathStyle;
        }, "second");

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredKeyedService<GenericS3Client>("first"));
        Assert.NotNull(provider.GetRequiredKeyedService<GenericS3Client>("second"));
        Assert.Null(provider.GetService<GenericS3Client>());
        Assert.Null(provider.GetService<ISimpleClient>());
    }

    [Fact]
    public void AddSimpleS3CoreKeepsNamedConfigOutOfDefaultOptionsConsumer()
    {
        ServiceCollection services = new ServiceCollection();

        SimpleS3CoreServices.AddSimpleS3Core(services, config =>
        {
            config.Endpoint = "https://named.example.com";
            config.RegionCode = "named-region";
            config.NamingMode = NamingMode.PathStyle;
            config.AlwaysCalculateContentMd5 = true;
        }, "custom");

        using ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<SimpleS3Config> options = provider.GetRequiredService<IOptionsMonitor<SimpleS3Config>>();
        SimpleS3Config namedConfig = options.Get("custom");

        Assert.Equal("https://named.example.com", namedConfig.Endpoint);
        Assert.Equal("named-region", namedConfig.RegionCode);
        Assert.True(namedConfig.AlwaysCalculateContentMd5);
        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<SimpleS3Config>>().Value);
    }

    [Fact]
    public void AddSimpleS3CoreNamedKeyedOptionsDoNotRequireAddOptionsSideEffect()
    {
        ServiceCollection services = new ServiceCollection();

        SimpleS3CoreServices.AddSimpleS3Core(services, name: "custom");
        services.AddSingleton<IConfigureOptions<SimpleS3Config>>(new ConfigureNamedOptions<SimpleS3Config>("custom", config =>
        {
            config.Endpoint = "https://manual.example.com";
            config.RegionCode = "manual-region";
            config.NamingMode = NamingMode.PathStyle;
        }));

        using ServiceProvider provider = services.BuildServiceProvider();
        SimpleS3Config config = provider.GetRequiredKeyedService<IOptions<SimpleS3Config>>("custom").Value;

        Assert.Equal("https://manual.example.com", config.Endpoint);
        Assert.Equal("manual-region", config.RegionCode);
    }

    [Fact]
    public void UseGenericS3AppliesNamedProviderConfigToCoreOptions()
    {
        ServiceCollection services = new ServiceCollection();
        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services, name: "custom");

        builder.UseGenericS3(config =>
        {
            config.Endpoint = "https://provider.example.com";
            config.RegionCode = "provider-region";
            config.NamingMode = NamingMode.PathStyle;
        });

        using ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<SimpleS3Config> options = provider.GetRequiredService<IOptionsMonitor<SimpleS3Config>>();
        SimpleS3Config namedConfig = options.Get("custom");

        Assert.Equal("GenericS3", namedConfig.ProviderName);
        Assert.Equal("https://provider.example.com", namedConfig.Endpoint);
        Assert.Equal("provider-region", namedConfig.RegionCode);
        Assert.Equal(NamingMode.PathStyle, namedConfig.NamingMode);
    }

    [Fact]
    public void AddGenericS3DefaultAndCustomKeepOptionsIsolated()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddGenericS3(config =>
        {
            config.Endpoint = "https://custom.example.com";
            config.RegionCode = "custom-region";
            config.NamingMode = NamingMode.PathStyle;
        }, "custom");

        services.AddGenericS3(config =>
        {
            config.Endpoint = "https://default.example.com";
            config.RegionCode = "default-region";
            config.NamingMode = NamingMode.PathStyle;
        });

        using ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<SimpleS3Config> options = provider.GetRequiredService<IOptionsMonitor<SimpleS3Config>>();

        Assert.Equal("https://custom.example.com", options.Get("custom").Endpoint);
        Assert.Equal("https://default.example.com", options.CurrentValue.Endpoint);
        Assert.Equal("https://custom.example.com", provider.GetRequiredKeyedService<IOptions<SimpleS3Config>>("custom").Value.Endpoint);
        Assert.Equal("https://default.example.com", provider.GetRequiredService<IOptions<SimpleS3Config>>().Value.Endpoint);
    }

    [Fact]
    public void AddGenericS3DefaultUsesLateUnkeyedInputValidatorOverride()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddGenericS3(config =>
        {
            config.Endpoint = "https://default.example.com";
            config.RegionCode = "default-region";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("short", "short");
        });
        services.AddSingleton<IInputValidator, StrictInputValidator>();

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<ISimpleClient>());
    }

    [Fact]
    public void NamedOptionsUseIsolatedInputValidators()
    {
        ServiceCollection services = new ServiceCollection();

        SimpleS3CoreServices.AddSimpleS3Core(services, config =>
        {
            config.Endpoint = "https://lenient.example.com";
            config.RegionCode = "lenient-region";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("short", "short");
        }, "lenient");
        services.AddKeyedSingleton<IInputValidator, LenientInputValidator>("lenient");

        SimpleS3CoreServices.AddSimpleS3Core(services, config =>
        {
            config.Endpoint = "https://strict.example.com";
            config.RegionCode = "strict-region";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("short", "short");
        }, "strict");
        services.AddKeyedSingleton<IInputValidator, StrictInputValidator>("strict");

        using ServiceProvider provider = services.BuildServiceProvider();
        IOptionsMonitor<SimpleS3Config> options = provider.GetRequiredService<IOptionsMonitor<SimpleS3Config>>();

        Assert.Equal("short", options.Get("lenient").Credentials?.KeyId);
        Assert.Throws<OptionsValidationException>(() => options.Get("strict"));
    }

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
        INetworkDriver driver = provider.GetRequiredKeyedService<INetworkDriver>("custom");
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

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = new List<Uri>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private class LenientInputValidator : InputValidatorBase
    {
        protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        protected override bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        protected override bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }
    }

    private sealed class StrictInputValidator : LenientInputValidator
    {
        protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
        {
            if (keyId.Length == 20)
            {
                status = ValidationStatus.Ok;
                message = null;
                return true;
            }

            status = ValidationStatus.WrongLength;
            message = "20";
            return false;
        }
    }
}