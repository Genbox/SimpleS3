using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;

public static class CoreBuilderExtensions
{
    public static ICoreBuilder UseGoogleCloudStorage(this ICoreBuilder clientBuilder, Action<GoogleCloudStorageConfig> config)
    {
        return UseGoogleCloudStorage(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
    }

    public static ICoreBuilder UseGoogleCloudStorage(this ICoreBuilder clientBuilder, Action<GoogleCloudStorageConfig, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(ServiceBuilderBase.GetOptionsName(clientBuilder.Name), config);
        return UseGoogleCloudStorage(clientBuilder);
    }

    public static ICoreBuilder UseGoogleCloudStorage(this ICoreBuilder clientBuilder)
    {
        clientBuilder.Services.AddKeyedSingleton<IRegionData, GoogleCloudStorageRegionData>(clientBuilder.Name);
        clientBuilder.Services.AddKeyedSingleton<IInputValidator, GoogleCloudStorageInputValidator>(clientBuilder.Name);

        if (clientBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            clientBuilder.Services.AddSingleton<IRegionData, GoogleCloudStorageRegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, GoogleCloudStorageInputValidator>();
        }

        clientBuilder.Services.PostConfigure<SimpleS3Config>(ServiceBuilderBase.GetOptionsName(clientBuilder.Name), (x, y) =>
        {
            IOptionsMonitor<GoogleCloudStorageConfig> awsCfg = y.GetRequiredService<IOptionsMonitor<GoogleCloudStorageConfig>>();
            PropertyHelper.MapObjects(awsCfg.Get(ServiceBuilderBase.GetOptionsName(clientBuilder.Name)), x);
        });

        return clientBuilder;
    }
}