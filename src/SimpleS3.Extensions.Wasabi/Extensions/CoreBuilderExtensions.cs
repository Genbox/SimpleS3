using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.Wasabi.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseWasabi(this ICoreBuilder clientBuilder, Action<WasabiConfig> config)
        {
            return UseWasabi(clientBuilder, (s3Config, _) => config.Invoke(s3Config));
        }

        public static ICoreBuilder UseWasabi(this ICoreBuilder clientBuilder, Action<WasabiConfig, IServiceProvider> config)
        {
            clientBuilder.Services.Configure(config);

            return UseWasabi(clientBuilder);
        }

        public static ICoreBuilder UseWasabi(this ICoreBuilder clientBuilder)
        {
            clientBuilder.Services.AddSingleton<IRegionData, WasabiRegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, WasabiInputValidator>();

            clientBuilder.Services.PostConfigure<Config>((x, y) =>
            {
                IOptions<WasabiConfig> awsCfg = y.GetRequiredService<IOptions<WasabiConfig>>();
                PropertyHelper.MapObjects(awsCfg.Value, x);
            });

            return clientBuilder;
        }
    }
}
