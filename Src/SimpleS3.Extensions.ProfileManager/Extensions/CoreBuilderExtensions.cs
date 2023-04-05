using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal;
using Genbox.SimpleS3.Extensions.ProfileManager.Serializers;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Genbox.SimpleS3.Extensions.ProfileManager.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public static class CoreBuilderExtensions
{
    /// <summary>Adds a profile manager that is configured to use the disk for storage and JSON for serialization</summary>
    public static IProfileManagerBuilder UseProfileManager(this ICoreBuilder builder, Action<DiskStorageOptions>? config = null)
    {
        builder.Services.AddSingleton<IProfileSetup, ConsoleProfileSetup>();
        builder.Services.AddSingleton<IProfileManager, ProfileManager>();
        builder.Services.AddSingleton<IStorage, DiskStorage>();
        builder.Services.AddSingleton<IProfileSerializer, JsonProfileSerializer>();

        if (config != null)
            builder.Services.Configure(config);

        return new ProfileManagerBuilder(builder.Services);
    }
}