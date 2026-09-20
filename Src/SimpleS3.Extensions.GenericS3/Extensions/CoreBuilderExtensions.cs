using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.GenericS3.Extensions;

/// <summary>Provides the <c>CoreBuilderExtensions</c> API.</summary>
public static class CoreBuilderExtensions
{
    /// <summary>Provides the <c>UseGenericS3</c> API.</summary>
    public static ICoreBuilder UseGenericS3(this ICoreBuilder clientBuilder, Action<GenericS3Config> config) => clientBuilder.UseGenericS3((s3Config, _) => config.Invoke(s3Config));

    /// <summary>Provides the <c>UseGenericS3</c> API.</summary>
    public static ICoreBuilder UseGenericS3(this ICoreBuilder clientBuilder, Action<GenericS3Config, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(config);

        return clientBuilder.UseGenericS3();
    }

    /// <summary>Provides the <c>UseGenericS3</c> API.</summary>
    public static ICoreBuilder UseGenericS3(this ICoreBuilder clientBuilder)
    {
        clientBuilder.Services.AddSingleton<IRegionData, GenericS3RegionData>();
        clientBuilder.Services.TryAddSingleton<IInputValidator, GenericS3InputValidator>();

        clientBuilder.Services.PostConfigure<SimpleS3Config>((x, y) =>
        {
            IOptions<GenericS3Config> awsCfg = y.GetRequiredService<IOptions<GenericS3Config>>();
            PropertyHelper.MapObjects(awsCfg.Value, x);
        });

        return clientBuilder;
    }
}