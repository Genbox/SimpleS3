using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.AwsS3.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseAwsS3(this ICoreBuilder clientBuilder, Action<AwsConfig> config)
        {
            return UseAwsS3(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
        }

        public static ICoreBuilder UseAwsS3(this ICoreBuilder clientBuilder, Action<AwsConfig, IServiceProvider> config)
        {
            clientBuilder.Services.Configure(config);

            return UseAwsS3(clientBuilder);
        }

        public static ICoreBuilder UseAwsS3(this ICoreBuilder clientBuilder)
        {
            clientBuilder.Services.AddSingleton<IRegionData, AwsRegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, AwsInputValidator>();
            clientBuilder.Services.AddSingleton<IUrlBuilder, AwsUrlBuilder>();

            clientBuilder.Services.PostConfigure<Config>((x, y) =>
            {
                IOptions<AwsConfig> awsCfg = y.GetRequiredService<IOptions<AwsConfig>>();
                PropertyMapper.MapObjects(awsCfg.Value, x);
            });

            return clientBuilder;
        }
    }
}