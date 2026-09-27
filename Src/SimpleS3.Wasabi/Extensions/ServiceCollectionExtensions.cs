using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.Wasabi;
using Genbox.SimpleS3.Extensions.Wasabi.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Wasabi.Extensions;

/// <summary>Provides Wasabi registration extensions.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds Wasabi services.</summary>
    public static IClientBuilder AddWasabi(this IServiceCollection services, Action<WasabiConfig, IServiceProvider> configure)
    {
        services.Configure(configure);
        return services.AddWasabi();
    }

    /// <summary>Adds Wasabi services.</summary>
    public static IClientBuilder AddWasabi(this IServiceCollection services, Action<WasabiConfig> configure)
    {
        services.Configure(configure);
        return services.AddWasabi();
    }

    /// <summary>Adds Wasabi services.</summary>
    public static IClientBuilder AddWasabi(this IServiceCollection services) => services.AddS3Provider(core => core.UseWasabi(),
        client => new WasabiClient(client));
}