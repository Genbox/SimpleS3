using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

/// <summary>Provides the <c>ConfigExtensions</c> API.</summary>
public static class ConfigExtensions
{
    /// <summary>Provides the <c>UseProfile</c> API.</summary>
    public static void UseProfile(this SimpleS3Config config, IProfile profile)
    {
        config.Credentials = new ProfileAccessKey(profile);
        config.RegionCode = profile.RegionCode;
    }

    /// <summary>Provides the <c>UseProfile</c> API.</summary>
    public static void UseProfile(this SimpleS3Config config, IProfileManager profileManager, string profileName)
    {
        IProfile? profile = profileManager.GetProfile(profileName);

        if (profile == null)
            throw new S3Exception($"The profile '{profileName}' does not exist.");

        config.Credentials = new ProfileAccessKey(profile);
        config.RegionCode = profile.RegionCode;
    }

    /// <summary>Provides the <c>UseDefaultProfile</c> API.</summary>
    public static void UseDefaultProfile(this SimpleS3Config config, IProfileManager profileManager) => config.UseProfile(profileManager, ProfileManager.DefaultProfile);
}