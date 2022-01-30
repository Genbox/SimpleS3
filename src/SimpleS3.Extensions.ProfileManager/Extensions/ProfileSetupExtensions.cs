using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public static class ProfileSetupExtensions
{
    public static IProfile SetupDefaultProfile(this IProfileSetup setup, bool persist = true) => setup.SetupProfile(ProfileManager.DefaultProfile, persist);
}