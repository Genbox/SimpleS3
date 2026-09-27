using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.ProviderBase;

/// <summary>Shared registration support for S3 providers.</summary>
internal static class ProviderServiceCollectionExtensions
{
    /// <summary>Registers the common client, transport, and retry services for a provider.</summary>
    public static IClientBuilder AddS3Provider<TClient>(this IServiceCollection services, Action<ICoreBuilder> configureProvider, Func<SimpleClient, TClient> clientFactory)
        where TClient : class, ISimpleClient
    {
        if (configureProvider == null)
            throw new ArgumentNullException(nameof(configureProvider));

        ServiceCollection providerServices = new ServiceCollection();

        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(providerServices, name: typeof(TClient).FullName ?? typeof(TClient).Name);
        configureProvider(coreBuilder);

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
        httpBuilder.UseRetryAndTimeout();

        services.AddSingleton(provider => new ProviderServiceHost<TClient>(providerServices, clientFactory, provider, services.Where(IsOptionsRegistration)));
        services.AddSingleton(provider => provider.GetRequiredService<ProviderServiceHost<TClient>>().Client);
        services.AddSingleton<ISimpleClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<IBucketClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<IObjectClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<IMultipartClient>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<IMultipartTransfer>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<ITransfer>(provider => provider.GetRequiredService<TClient>());
        services.AddSingleton<ISignedClient>(provider => provider.GetRequiredService<TClient>());

        return new ClientBuilder(providerServices, httpBuilder, coreBuilder);
    }

    private static bool IsOptionsRegistration(ServiceDescriptor descriptor)
    {
        if (!descriptor.ServiceType.IsGenericType)
            return false;

        Type definition = descriptor.ServiceType.GetGenericTypeDefinition();
        return definition == typeof(IConfigureOptions<>) || definition == typeof(IPostConfigureOptions<>) || definition == typeof(IValidateOptions<>);
    }

    private sealed class ProviderServiceHost<TClient> : IDisposable where TClient : class, ISimpleClient
    {
        public ProviderServiceHost(IServiceCollection services, Func<SimpleClient, TClient> clientFactory, IServiceProvider applicationServices, IEnumerable<ServiceDescriptor> applicationOptions)
        {
            ServiceCollection isolatedServices = new ServiceCollection();

            foreach (IGrouping<Type, ServiceDescriptor> registrations in applicationOptions.GroupBy(x => x.ServiceType))
            {
                if (registrations.Key.ContainsGenericParameters)
                {
                    foreach (ServiceDescriptor registration in registrations)
                        isolatedServices.Add(registration);
                    continue;
                }

                // Resolve in the application container so callback dependencies and ownership stay there.
                foreach (object? options in applicationServices.GetServices(registrations.Key).Take(registrations.Count()))
                    isolatedServices.Add(ServiceDescriptor.Singleton(registrations.Key, options!));
            }

            foreach (ServiceDescriptor descriptor in services)
                isolatedServices.Add(descriptor);

            INetworkDriver? networkDriver = applicationServices.GetService<INetworkDriver>();
            if (networkDriver != null)
                isolatedServices.Replace(ServiceDescriptor.Singleton(networkDriver));

            ILoggerFactory? loggerFactory = applicationServices.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                isolatedServices.Replace(ServiceDescriptor.Singleton(loggerFactory));

            ServiceProvider isolatedProvider = isolatedServices.BuildServiceProvider();

            try
            {
                Client = clientFactory((SimpleClient)isolatedProvider.GetRequiredService<ISimpleClient>());
                Services = isolatedProvider;
            }
            catch
            {
                isolatedProvider.Dispose();
                throw;
            }
        }

        public ServiceProvider Services { get; }
        public TClient Client { get; }

        public void Dispose() => Services.Dispose();
    }
}