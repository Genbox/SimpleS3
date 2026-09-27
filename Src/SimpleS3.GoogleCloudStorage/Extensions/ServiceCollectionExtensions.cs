using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GoogleCloudStorage.Extensions;

/// <summary>Provides Google Cloud Storage registration extensions.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds Google Cloud Storage services.</summary>
    public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection services, Action<GoogleCloudStorageConfig, IServiceProvider> configure)
    {
        services.Configure(configure);
        return services.AddGoogleCloudStorage();
    }

    /// <summary>Adds Google Cloud Storage services.</summary>
    public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection services, Action<GoogleCloudStorageConfig> configure)
    {
        services.Configure(configure);
        return services.AddGoogleCloudStorage();
    }

    /// <summary>Adds Google Cloud Storage services.</summary>
    public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection services) => services.AddS3Provider(core => core.UseGoogleCloudStorage(),
        client => new GoogleCloudStorageClient(client));
}