using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Extensions;

/// <summary>Provides the <c>CoreBuilderExtensions</c> API.</summary>
public static class CoreBuilderExtensions
{
    /// <summary>Provides the <c>UseAmazonS3</c> API.</summary>
    public static ICoreBuilder UseAmazonS3(this ICoreBuilder clientBuilder, Action<AmazonS3Config> config) => clientBuilder.UseAmazonS3((s3Config, _) => config.Invoke(s3Config));

    /// <summary>Provides the <c>UseAmazonS3</c> API.</summary>
    public static ICoreBuilder UseAmazonS3(this ICoreBuilder clientBuilder, Action<AmazonS3Config, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(config);

        return clientBuilder.UseAmazonS3();
    }

    /// <summary>Provides the <c>UseAmazonS3</c> API.</summary>
    public static ICoreBuilder UseAmazonS3(this ICoreBuilder clientBuilder)
    {
        clientBuilder.Services.AddSingleton<IRegionData, AmazonS3RegionData>();
        clientBuilder.Services.AddSingleton<IInputValidator, AmazonS3InputValidator>();

        clientBuilder.Services.PostConfigure<SimpleS3Config>((x, y) =>
        {
            IOptions<AmazonS3Config> awsCfg = y.GetRequiredService<IOptions<AmazonS3Config>>();
            PropertyHelper.MapObjects(awsCfg.Value, x);
        });

        return clientBuilder;
    }
}