using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public static class ProfileManagerExtensions
{
    public static IProfile? GetDefaultProfile(this IProfileManager profileManager) => profileManager.GetProfile(ProfileManager.DefaultProfile);
}