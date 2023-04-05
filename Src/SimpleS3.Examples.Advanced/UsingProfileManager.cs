using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.AmazonS3.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.Extensions.ProfileManager;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Serializers;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Genbox.SimpleS3.Extensions.ProfileManager.Storage;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Examples.Advanced;

public static class UsingProfileManager
{
    /// <summary>
    /// ProfileManager can handle credentials securely. This example shows how to use it via DI.
    /// </summary>
    public static void ExampleWithDependencyInjection()
    {
        ServiceCollection collection = new ServiceCollection();
        IClientBuilder builder = collection.AddAmazonS3();

        //Here we add the profile manager. It is a profile system that persist your credentials to disk in a very secure way.
        builder.CoreBuilder
               .UseProfileManager()
               .BindConfigToDefaultProfile() //We can either name the profile (so you can have more than one) or use the default one.
               .UseConsoleSetup(); //This adds a service that ask you to setup your profile if it does not exist.

        using ServiceProvider provider = collection.BuildServiceProvider();

        IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();
        IProfile? profile = profileManager.GetDefaultProfile();

        if (profile == null)
        {
            IProfileSetup setup = provider.GetRequiredService<IProfileSetup>();
            setup.SetupDefaultProfile();
        }

        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
    }

    /// <summary>
    /// ProfileManager can handle credentials securely. This example shows how to use it via a normal client.
    /// </summary>
    public static void ExampleWithClient()
    {
        AmazonS3InputValidator validator = new AmazonS3InputValidator();
        JsonProfileSerializer serializer = new JsonProfileSerializer();

        IOptions<DiskStorageOptions> storageOptions = Options.Create(new DiskStorageOptions());
        DiskStorage storage = new DiskStorage(storageOptions);

        IOptions<ProfileManagerOptions> profileOptions = Options.Create(new ProfileManagerOptions());
        ProfileManager profileManager = new ProfileManager(validator, serializer, storage, profileOptions);

        IProfile? profile = profileManager.GetDefaultProfile();

        if (profile == null)
        {
            RegionConverter converter = new RegionConverter(AmazonS3RegionData.Instance);
            IProfileSetup setup = new ConsoleProfileSetup(profileManager, validator, converter, AmazonS3RegionData.Instance);
            profile = setup.SetupDefaultProfile();
        }

        AmazonS3Config clientConfig = new AmazonS3Config();
        clientConfig.UseProfile(profile);

        using AmazonS3Client client = new AmazonS3Client(clientConfig);
    }
}