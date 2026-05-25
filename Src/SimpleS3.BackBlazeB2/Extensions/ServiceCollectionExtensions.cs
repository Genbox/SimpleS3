using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core;
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
    public static IClientBuilder AddBackBlazeB2(this IServiceCollection collection, Action<BackBlazeB2Config, IServiceProvider> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddBackBlazeB2(collection, name);
    }

    public static IClientBuilder AddBackBlazeB2(this IServiceCollection collection, Action<BackBlazeB2Config> config, string name = ServiceBuilderBase.DefaultName)
    {
        collection.Configure(ServiceBuilderBase.GetOptionsName(name), config);
        return AddBackBlazeB2(collection, name);
    }

    public static IClientBuilder AddBackBlazeB2(this IServiceCollection collection, string name = ServiceBuilderBase.DefaultName)
    {
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection, name: name);
        coreBuilder.UseBackBlazeB2();

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
        httpBuilder.UseRetryAndTimeout();

        coreBuilder.Services.AddKeyedSingleton<BackBlazeB2Client>(coreBuilder.Name, (x, _) => new BackBlazeB2Client((SimpleClient)x.GetRequiredKeyedService<ISimpleClient>(coreBuilder.Name)));

        if (coreBuilder.Name == ServiceBuilderBase.DefaultName)
        {
            coreBuilder.Services.AddSingleton(x => x.GetRequiredKeyedService<BackBlazeB2Client>(coreBuilder.Name));

            //Add the client as the interface too
            coreBuilder.Services.AddSingleton<ISimpleClient>(x => x.GetRequiredKeyedService<BackBlazeB2Client>(coreBuilder.Name));
        }

        return new ClientBuilder(collection, httpBuilder, coreBuilder, coreBuilder.Name);
    }
}