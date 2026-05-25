using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.Wasabi;
using Genbox.SimpleS3.Extensions.Wasabi.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Wasabi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The configuration delegate</param>
    public static IClientBuilder AddWasabi(this IServiceCollection collection, Action<WasabiConfig, IServiceProvider> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddWasabi(collection, name);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The configuration delegate</param>
    public static IClientBuilder AddWasabi(this IServiceCollection collection, Action<WasabiConfig> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddWasabi(collection, name);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    public static IClientBuilder AddWasabi(this IServiceCollection collection, string name = ServiceBuilderBase.DefaultName)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection, name: name);
        coreBuilder.UseWasabi();

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
        httpBuilder.UseRetryAndTimeout();

        coreBuilder.Services.AddKeyedSingleton<WasabiClient>(coreBuilder.Name, (x, _) => new WasabiClient((SimpleClient)x.GetRequiredKeyedService<ISimpleClient>(coreBuilder.Name)));

        if (coreBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            coreBuilder.Services.AddSingleton(x => x.GetRequiredKeyedService<WasabiClient>(coreBuilder.Name));

            //Add the client as the interface too
            coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredKeyedService<WasabiClient>(coreBuilder.Name));
        }

        return new ClientBuilder(collection, httpBuilder, coreBuilder, coreBuilder.Name);
    }
}