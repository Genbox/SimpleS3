using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
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
    public static IClientBuilder AddGenericS3(this IServiceCollection collection, Action<GenericS3Config, IServiceProvider> config)
    {
        collection.Configure(config);
        return AddGenericS3(collection);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    /// <param name="config">The configuration delegate</param>
    public static IClientBuilder AddGenericS3(this IServiceCollection collection, Action<GenericS3Config> config)
    {
        collection.Configure(config);
        return AddGenericS3(collection);
    }

    /// <summary>Add SimpleS3 services to a service collection.</summary>
    /// <param name="collection">The service collection</param>
    public static IClientBuilder AddGenericS3(this IServiceCollection collection)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        coreBuilder.UseAmazonS3();

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
            return new GenericS3Client(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
        });

        //Add the client as the interface too
        coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredService<GenericS3Client>());

        return new ClientBuilder(collection, httpBuilder, coreBuilder);
    }
}