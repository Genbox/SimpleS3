using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.GenericS3.Extensions;

public static class CoreBuilderExtensions
{
    public static ICoreBuilder UseGenericS3(this ICoreBuilder clientBuilder, Action<GenericS3Config> config)
    {
        return UseGenericS3(clientBuilder, (s3Config, _) => config.Invoke(s3Config));
    }

    public static ICoreBuilder UseGenericS3(this ICoreBuilder clientBuilder, Action<GenericS3Config, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(ServiceBuilderBase.GetOptionsName(clientBuilder.Name), config);

        return UseGenericS3(clientBuilder);
    }

    public static ICoreBuilder UseGenericS3(this ICoreBuilder clientBuilder)
    {
        clientBuilder.Services.AddKeyedSingleton<IRegionData, GenericS3RegionData>(clientBuilder.Name);
        clientBuilder.Services.AddKeyedSingleton<IInputValidator, GenericS3InputValidator>(clientBuilder.Name);

        if (clientBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            clientBuilder.Services.AddSingleton<IRegionData, GenericS3RegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, GenericS3InputValidator>();
        }

        clientBuilder.Services.PostConfigure<SimpleS3Config>(ServiceBuilderBase.GetOptionsName(clientBuilder.Name), (x, y) =>
        {
            IOptionsMonitor<GenericS3Config> awsCfg = y.GetRequiredService<IOptionsMonitor<GenericS3Config>>();
            PropertyHelper.MapObjects(awsCfg.Get(ServiceBuilderBase.GetOptionsName(clientBuilder.Name)), x);
        });

        return clientBuilder;
    }
}