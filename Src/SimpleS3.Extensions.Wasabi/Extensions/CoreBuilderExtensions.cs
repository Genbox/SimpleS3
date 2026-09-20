using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.Wasabi.Extensions;

/// <summary>Provides the <c>CoreBuilderExtensions</c> API.</summary>
public static class CoreBuilderExtensions
{
    /// <summary>Provides the <c>UseWasabi</c> API.</summary>
    public static ICoreBuilder UseWasabi(this ICoreBuilder clientBuilder, Action<WasabiConfig> config) => clientBuilder.UseWasabi((s3Config, _) => config.Invoke(s3Config));

    /// <summary>Provides the <c>UseWasabi</c> API.</summary>
    public static ICoreBuilder UseWasabi(this ICoreBuilder clientBuilder, Action<WasabiConfig, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(config);

        return clientBuilder.UseWasabi();
    }

    /// <summary>Provides the <c>UseWasabi</c> API.</summary>
    public static ICoreBuilder UseWasabi(this ICoreBuilder clientBuilder)
    {
        clientBuilder.Services.AddSingleton<IRegionData, WasabiRegionData>();
        clientBuilder.Services.AddSingleton<IInputValidator, WasabiInputValidator>();

        clientBuilder.Services.PostConfigure<SimpleS3Config>((x, y) =>
        {
            IOptions<WasabiConfig> awsCfg = y.GetRequiredService<IOptions<WasabiConfig>>();
            PropertyHelper.MapObjects(awsCfg.Value, x);
        });

        return clientBuilder;
    }
}