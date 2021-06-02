using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder, Action<BackBlazeB2Config> config)
        {
            return UseBackBlazeB2(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
        }

        public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder, Action<BackBlazeB2Config, IServiceProvider> config)
        {
            clientBuilder.Services.Configure(config);
            return UseBackBlazeB2(clientBuilder);
        }

        public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder)
        {
            clientBuilder.Services.AddSingleton<IRegionData, BackblazeB2RegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, BackblazeB2InputValidator>();
            clientBuilder.Services.AddSingleton<IUrlBuilder, BackblazeB2UrlBuilder>();

            clientBuilder.Services.PostConfigure<Config>((x, y) =>
            {
                IOptions<BackBlazeB2Config> awsCfg = y.GetRequiredService<IOptions<BackBlazeB2Config>>();
                PropertyHelper.MapObjects(awsCfg.Value, x);
            });

            return clientBuilder;
        }
    }
}