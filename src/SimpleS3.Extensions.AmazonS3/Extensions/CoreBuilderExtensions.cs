using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseAmazonS3(this ICoreBuilder clientBuilder, Action<AmazonS3Config> config)
        {
            return UseAmazonS3(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
        }

        public static ICoreBuilder UseAmazonS3(this ICoreBuilder clientBuilder, Action<AmazonS3Config, IServiceProvider> config)
        {
            clientBuilder.Services.Configure(config);

            return UseAmazonS3(clientBuilder);
        }

        public static ICoreBuilder UseAmazonS3(this ICoreBuilder clientBuilder)
        {
            clientBuilder.Services.AddSingleton<IRegionData, AmazonS3RegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, AmazonS3InputValidator>();
            clientBuilder.Services.AddSingleton<IUrlBuilder, AmazonS3UrlBuilder>();

            clientBuilder.Services.PostConfigure<Config>((x, y) =>
            {
                IOptions<AmazonS3Config> awsCfg = y.GetRequiredService<IOptions<AmazonS3Config>>();
                PropertyHelper.MapObjects(awsCfg.Value, x);
            });

            return clientBuilder;
        }
    }
}