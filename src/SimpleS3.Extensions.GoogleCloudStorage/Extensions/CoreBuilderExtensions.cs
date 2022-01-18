using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseGoogleCloudStorage(this ICoreBuilder clientBuilder, Action<GoogleCloudStorageConfig> config)
        {
            return UseGoogleCloudStorage(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
        }

        public static ICoreBuilder UseGoogleCloudStorage(this ICoreBuilder clientBuilder, Action<GoogleCloudStorageConfig, IServiceProvider> config)
        {
            clientBuilder.Services.Configure(config);
            return UseGoogleCloudStorage(clientBuilder);
        }

        public static ICoreBuilder UseGoogleCloudStorage(this ICoreBuilder clientBuilder)
        {
            clientBuilder.Services.AddSingleton<IRegionData, GoogleCloudStorageRegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, GoogleCloudStorageValidator>();

            clientBuilder.Services.PostConfigure<SimpleS3Config>((x, y) =>
            {
                IOptions<GoogleCloudStorageConfig> awsCfg = y.GetRequiredService<IOptions<GoogleCloudStorageConfig>>();
                PropertyHelper.MapObjects(awsCfg.Value, x);
            });

            return clientBuilder;
        }
    }
}