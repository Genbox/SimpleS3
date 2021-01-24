using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder, Action<B2Config> config)
        {
            return UseBackBlazeB2(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
        }

        public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder, Action<B2Config, IServiceProvider> config)
        {
            clientBuilder.Services.Configure(config);
            return UseBackBlazeB2(clientBuilder);
        }

        public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder)
        {
            clientBuilder.Services.AddSingleton<IRegionData, B2RegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, B2InputValidator>();
            clientBuilder.Services.AddSingleton<IUrlBuilder, B2UrlBuilder>();

            clientBuilder.Services.PostConfigure<Config>((x, y) =>
            {
                IOptions<B2Config> awsCfg = y.GetRequiredService<IOptions<B2Config>>();
                PropertyMapper.MapObjects(awsCfg.Value, x);
            });

            return clientBuilder;
        }
    }
}