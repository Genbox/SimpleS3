using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.BackBlazeB2;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.BackBlazeB2.Extensions;

/// <summary>Provides Backblaze B2 registration extensions.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds Backblaze B2 services.</summary>
    public static IClientBuilder AddBackBlazeB2(this IServiceCollection services, Action<BackBlazeB2Config, IServiceProvider> configure)
    {
        services.Configure(configure);
        return services.AddBackBlazeB2();
    }

    /// <summary>Adds Backblaze B2 services.</summary>
    public static IClientBuilder AddBackBlazeB2(this IServiceCollection services, Action<BackBlazeB2Config> configure)
    {
        services.Configure(configure);
        return services.AddBackBlazeB2();
    }

    /// <summary>Adds Backblaze B2 services.</summary>
    public static IClientBuilder AddBackBlazeB2(this IServiceCollection services) => services.AddS3Provider(core => core.UseBackBlazeB2(), client => new BackBlazeB2Client(client));
}