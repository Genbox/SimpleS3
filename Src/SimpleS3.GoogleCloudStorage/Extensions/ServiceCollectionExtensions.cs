using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GoogleCloudStorage.Extensions;

public static class ServiceCollectionExtensions
{
    public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection collection, Action<GoogleCloudStorageConfig, IServiceProvider> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddGoogleCloudStorage(collection, name);
    }

    public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection collection, Action<GoogleCloudStorageConfig> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddGoogleCloudStorage(collection, name);
    }

    public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection collection, string name = ServiceBuilderBase.DefaultName)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection, name: name);
        coreBuilder.UseGoogleCloudStorage();

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
        httpBuilder.UseRetryAndTimeout();

        coreBuilder.Services.AddKeyedSingleton<GoogleCloudStorageClient>(coreBuilder.Name, (x, _) => new GoogleCloudStorageClient((SimpleClient)x.GetRequiredKeyedService<ISimpleClient>(coreBuilder.Name)));

        if (coreBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            coreBuilder.Services.AddSingleton(x => x.GetRequiredKeyedService<GoogleCloudStorageClient>(coreBuilder.Name));

            //Add the client as the interface too
            coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredKeyedService<GoogleCloudStorageClient>(coreBuilder.Name));
        }

        return new ClientBuilder(collection, httpBuilder, coreBuilder, coreBuilder.Name);
    }
}