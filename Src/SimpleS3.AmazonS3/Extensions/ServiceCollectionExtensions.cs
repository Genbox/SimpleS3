using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.AmazonS3.Extensions;

/// <summary>Provides Amazon S3 registration extensions.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds Amazon S3 services.</summary>
    public static IClientBuilder AddAmazonS3(this IServiceCollection services, Action<AmazonS3Config, IServiceProvider> configure)
    {
        services.Configure(configure);
        return services.AddAmazonS3();
    }

    /// <summary>Adds Amazon S3 services.</summary>
    public static IClientBuilder AddAmazonS3(this IServiceCollection services, Action<AmazonS3Config> configure)
    {
        services.Configure(configure);
        return services.AddAmazonS3();
    }

    /// <summary>Adds Amazon S3 services.</summary>
    public static IClientBuilder AddAmazonS3(this IServiceCollection services) => services.AddS3Provider(core => core.UseAmazonS3(), client => new AmazonS3Client(client));
}