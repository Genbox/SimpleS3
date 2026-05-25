using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.GenericS3;
using Genbox.SimpleS3.Extensions.GenericS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GenericS3.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The configuration delegate</param>
    public static IClientBuilder AddGenericS3(this IServiceCollection collection, Action<GenericS3Config, IServiceProvider> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddGenericS3(collection, name);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The configuration delegate</param>
    public static IClientBuilder AddGenericS3(this IServiceCollection collection, Action<GenericS3Config> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddGenericS3(collection, name);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    public static IClientBuilder AddGenericS3(this IServiceCollection collection, string name = ServiceBuilderBase.DefaultName)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection, name: name);
        coreBuilder.UseGenericS3();

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
        httpBuilder.UseRetryAndTimeout();

        coreBuilder.Services.AddKeyedSingleton<GenericS3Client>(coreBuilder.Name, (x, _) => new GenericS3Client((SimpleClient)x.GetRequiredKeyedService<ISimpleClient>(coreBuilder.Name)));

        if (coreBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            coreBuilder.Services.AddSingleton(x => x.GetRequiredKeyedService<GenericS3Client>(coreBuilder.Name));

            //Add the client as the interface too
            coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredKeyedService<GenericS3Client>(coreBuilder.Name));
        }

        return new ClientBuilder(collection, httpBuilder, coreBuilder, coreBuilder.Name);
    }
}