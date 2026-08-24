using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

/// <summary>Provides the <c>ProfileManagerExtensions</c> API.</summary>
public static class ProfileManagerExtensions
{
    /// <summary>Provides the <c>GetDefaultProfile</c> API.</summary>
    public static IProfile? GetDefaultProfile(this IProfileManager profileManager) => profileManager.GetProfile(ProfileManager.DefaultProfile);
}