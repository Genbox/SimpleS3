using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;

public static class CoreBuilderExtensions
{
    public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder, Action<BackBlazeB2Config> config)
    {
        return UseBackBlazeB2(clientBuilder, (b2Config, _) => config.Invoke(b2Config));
    }

    public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder, Action<BackBlazeB2Config, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(ServiceBuilderBase.GetOptionsName(clientBuilder.Name), config);
        return UseBackBlazeB2(clientBuilder);
    }

    public static ICoreBuilder UseBackBlazeB2(this ICoreBuilder clientBuilder)
    {
        clientBuilder.Services.AddKeyedSingleton<IRegionData, BackblazeB2RegionData>(clientBuilder.Name);
        clientBuilder.Services.AddKeyedSingleton<IInputValidator, BackblazeB2InputValidator>(clientBuilder.Name);

        if (clientBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            clientBuilder.Services.AddSingleton<IRegionData, BackblazeB2RegionData>();
            clientBuilder.Services.AddSingleton<IInputValidator, BackblazeB2InputValidator>();
        }

        clientBuilder.Services.PostConfigure<SimpleS3Config>(ServiceBuilderBase.GetOptionsName(clientBuilder.Name), (x, y) =>
        {
            IOptionsMonitor<BackBlazeB2Config> awsCfg = y.GetRequiredService<IOptionsMonitor<BackBlazeB2Config>>();
            PropertyHelper.MapObjects(awsCfg.Get(ServiceBuilderBase.GetOptionsName(clientBuilder.Name)), x);
        });

        return clientBuilder;
    }
}