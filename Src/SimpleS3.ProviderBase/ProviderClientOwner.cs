using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.ProviderBase;

/// <summary>Owns the services created for a standalone provider client.</summary>
internal sealed class ProviderClientOwner : IDisposable
{
    private readonly ServiceProvider _services;

    internal ProviderClientOwner(IInputValidator inputValidator, SimpleS3Config config, NetworkConfig? networkConfig = null, INetworkDriver? networkDriver = null)
    {
        ServiceCollection services = CreateServices(inputValidator, config);
        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

        if (networkDriver != null)
            services.AddSingleton(networkDriver);
        else
        {
            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();
            networkConfig ??= new NetworkConfig();

            if (networkConfig.Proxy != null)
                httpBuilder.UseProxy(networkConfig.Proxy);

            httpBuilder.UseRetryAndTimeout(options =>
            {
                options.Retries = networkConfig.Retries;
                options.RetryMode = networkConfig.RetryMode;
                options.Timeout = networkConfig.Timeout;
                options.MaxRandomDelay = networkConfig.MaxRandomDelay;
            });
        }

        _services = services.BuildServiceProvider();

        try
        {
            Client = (SimpleClient)_services.GetRequiredService<ISimpleClient>();
        }
        catch
        {
            _services.Dispose();
            throw;
        }
    }

    internal SimpleClient Client { get; }

    public void Dispose() => _services.Dispose();

    private static ServiceCollection CreateServices(IInputValidator inputValidator, SimpleS3Config config)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton(inputValidator);
        services.AddSingleton<IOptions<SimpleS3Config>>(provider =>
        {
            List<string> failures = [];

            foreach (IValidateOptions<SimpleS3Config> validator in provider.GetServices<IValidateOptions<SimpleS3Config>>())
            {
                ValidateOptionsResult result = validator.Validate(Options.DefaultName, config);
                if (result.Failed)
                    failures.AddRange(result.Failures);
            }

            if (failures.Count != 0)
                throw new OptionsValidationException(Options.DefaultName, typeof(SimpleS3Config), failures);
            return Options.Create(config);
        });
        services.AddLogging();
        return services;
    }
}