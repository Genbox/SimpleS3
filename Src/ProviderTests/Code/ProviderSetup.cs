using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Extensions.HttpClientFactory;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.ProviderTests.Code;

internal sealed class ProviderSetup : IDisposable
{
    private readonly string auditDirectory = Path.Combine(Path.Combine(AppContext.BaseDirectory, "NetworkAudit"), $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
    private readonly Dictionary<S3Provider, ServiceProvider> providers = [];
    private readonly List<ServiceProvider> serviceProviders = [];

    private ProviderSetup()
    {
        BuildProvider(S3Provider.AmazonS3);
        BuildProvider(S3Provider.BackBlazeB2);
        BuildProvider(S3Provider.GoogleCloudStorage);
        BuildProvider(S3Provider.Wasabi);
    }

    public static ProviderSetup Instance { get; } = new ProviderSetup();
    public IList<(S3Provider, IProfile, ISimpleClient)> Clients { get; } = new List<(S3Provider, IProfile, ISimpleClient)>();

    public void Dispose()
    {
        foreach (ServiceProvider serviceProvider in serviceProviders)
            serviceProvider.Dispose();
    }

    public IProfile GetProfile(S3Provider provider) => Clients.First(x => x.Item1 == provider).Item2;

    public IAccessKeyProtector GetAccessKeyProtector(S3Provider provider) => providers[provider].GetRequiredService<IAccessKeyProtector>();

    private void BuildProvider(S3Provider provider)
    {
        ServiceProvider services = UtilityHelper.CreateSimpleS3(provider, "TestSetup-" + provider, false, c => c.ObjectKeyValidationMode = ObjectKeyValidationMode.Disabled, collection =>
        {
            ServiceDescriptor transport = collection.Last(x => x.ServiceType == typeof(INetworkDriver));
            collection.Remove(transport);
            collection.AddSingleton(sp => (HttpClientFactoryNetworkDriver)transport.ImplementationFactory!(sp));
            collection.AddSingleton<INetworkDriver>(sp => new AuditingNetworkDriver(sp.GetRequiredService<HttpClientFactoryNetworkDriver>(), Path.Combine(auditDirectory, provider.ToString())));
        });

        IProfileManager profileManager = services.GetRequiredService<IProfileManager>();

        IProfile? profile = profileManager.GetProfile("TestSetup-" + provider);

        if (profile != null)
        {
            serviceProviders.Add(services);
            providers.Add(provider, services);
            Clients.Add((provider, profile, services.GetRequiredService<ISimpleClient>()));
        }
        else
            services.Dispose();
    }
}