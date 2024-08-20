using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.BackBlazeB2;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.BackBlazeB2.Extensions;

public static class ServiceCollectionExtensions
{
    public static IClientBuilder AddBackBlazeB2(this IServiceCollection collection, Action<BackBlazeB2Config, IServiceProvider> config)
    {
        collection.Configure(config);
        return AddBackBlazeB2(collection);
    }

    public static IClientBuilder AddBackBlazeB2(this IServiceCollection collection, Action<BackBlazeB2Config> config)
    {
        collection.Configure(config);
        return AddBackBlazeB2(collection);
    }

    public static IClientBuilder AddBackBlazeB2(this IServiceCollection collection)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        coreBuilder.UseBackBlazeB2();

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
            return new BackBlazeB2Client(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
        });

        //Add the client as the interface too
        coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredService<BackBlazeB2Client>());

        return new ClientBuilder(collection, httpBuilder, coreBuilder);
    }
}