using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
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
    public static IClientBuilder AddWasabi(this IServiceCollection collection, Action<WasabiConfig, IServiceProvider> config)
    {
        collection.Configure(config);
        return AddWasabi(collection);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The configuration delegate</param>
    public static IClientBuilder AddWasabi(this IServiceCollection collection, Action<WasabiConfig> config)
    {
        collection.Configure(config);
        return AddWasabi(collection);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    public static IClientBuilder AddWasabi(this IServiceCollection collection)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        coreBuilder.UseWasabi();

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
        httpBuilder.UseRetryAndTimeout();

        coreBuilder.Services.AddSingleton(x =>
        {
            //We have to call a specific constructor for dependency injection
            IObjectClient objectClient = x.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = x.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = x.GetRequiredService<IMultipartClient>();
            IMultipartTransfer multipartTransfer = x.GetRequiredService<IMultipartTransfer>();
            ITransfer transfer = x.GetRequiredService<ITransfer>();
            ISignedClient signedObjectClient = x.GetRequiredService<ISignedClient>();
            return new WasabiClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
        });

        //Add the client as the interface too
        coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredService<WasabiClient>());

        return new ClientBuilder(collection, httpBuilder, coreBuilder);
    }
}