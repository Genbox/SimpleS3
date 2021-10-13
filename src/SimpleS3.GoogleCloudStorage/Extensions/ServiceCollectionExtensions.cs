#if COMMERCIAL
using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.GoogleCloudStorage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection collection, Action<GoogleCloudStorageConfig, IServiceProvider> config)
        {
            collection.Configure(config);
            return AddGoogleCloudStorage(collection);
        }

        public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection collection, Action<GoogleCloudStorageConfig> config)
        {
            collection.Configure(config);
            return AddGoogleCloudStorage(collection);
        }

        public static IClientBuilder AddGoogleCloudStorage(this IServiceCollection collection)
        {
            ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection);
            coreBuilder.UseGoogleCloudStorage();

            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
            httpBuilder.UseDefaultHttpPolicy();

            coreBuilder.Services.AddSingleton(x =>
            {
                //We have to call a specific constructor for dependency injection
                IObjectClient objectClient = x.GetRequiredService<IObjectClient>();
                IBucketClient bucketClient = x.GetRequiredService<IBucketClient>();
                IMultipartClient multipartClient = x.GetRequiredService<IMultipartClient>();
                IMultipartTransfer multipartTransfer = x.GetRequiredService<IMultipartTransfer>();
                ITransfer transfer = x.GetRequiredService<ITransfer>();
                ISignedObjectClient? signedObjectClient = x.GetRequiredService<ISignedObjectClient>();
                return new GoogleCloudStorageClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
            });

            //Add the client as the interface too
            coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredService<GoogleCloudStorageClient>());

            return new ClientBuilder(collection, httpBuilder, coreBuilder);
        }
    }
}
#endif