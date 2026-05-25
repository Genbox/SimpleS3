using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.DataProtection;
using Genbox.SimpleS3.Extensions.ProfileManager.Serializers;
using Genbox.SimpleS3.Extensions.ProfileManager.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public static class CoreBuilderExtensions
{
    /// <summary>Adds a profile manager that is configured to use the disk for storage and JSON for serialization</summary>
    public static IProfileManagerBuilder UseProfileManager(this ICoreBuilder builder, Action<DiskStorageOptions>? config = null)
    {
        builder.Services.AddDataProtection(options => options.ApplicationDiscriminator = "SimpleS3");
        builder.Services.TryAddSingleton<IAccessKeyProtector, DataProtectionKeyProtector>();
        builder.Services.TryAddSingleton<IProfileManager, ProfileManager>();
        builder.Services.TryAddSingleton<IStorage, DiskStorage>();
        builder.Services.TryAddSingleton<IProfileSerializer, DefaultProfileSerializer>();

        if (config != null)
            builder.Services.Configure(config);

        return new ProfileManagerBuilder(builder.Services, ServiceBuilderBase.GetOptionsName(builder.Name));
    }
}