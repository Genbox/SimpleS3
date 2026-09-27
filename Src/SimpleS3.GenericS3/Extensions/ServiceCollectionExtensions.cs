using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.GenericS3;
using Genbox.SimpleS3.Extensions.GenericS3.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GenericS3.Extensions;

/// <summary>Provides generic S3 registration extensions.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds generic S3 services.</summary>
    public static IClientBuilder AddGenericS3(this IServiceCollection services, Action<GenericS3Config, IServiceProvider> configure)
    {
        services.Configure(configure);
        return services.AddGenericS3();
    }

    /// <summary>Adds generic S3 services.</summary>
    public static IClientBuilder AddGenericS3(this IServiceCollection services, Action<GenericS3Config> configure)
    {
        services.Configure(configure);
        return services.AddGenericS3();
    }

    /// <summary>Adds generic S3 services.</summary>
    public static IClientBuilder AddGenericS3(this IServiceCollection services) => services.AddS3Provider(core => core.UseGenericS3(),
        client => new GenericS3Client(client));
}