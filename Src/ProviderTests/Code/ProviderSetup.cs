using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.ProviderTests.Code;

internal sealed class ProviderSetup : IDisposable
{
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

    private void BuildProvider(S3Provider provider)
    {
        ServiceProvider services = UtilityHelper.CreateSimpleS3(provider, "TestSetup-" + provider, false, c => c.ObjectKeyValidationMode = ObjectKeyValidationMode.Disabled);

        IProfileManager profileManager = services.GetRequiredService<IProfileManager>();

        IProfile? profile = profileManager.GetProfile("TestSetup-" + provider);

        if (profile != null)
        {
            serviceProviders.Add(services);
            Clients.Add((provider, profile, services.GetRequiredService<ISimpleClient>()));
        }
        else
            services.Dispose();
    }
}