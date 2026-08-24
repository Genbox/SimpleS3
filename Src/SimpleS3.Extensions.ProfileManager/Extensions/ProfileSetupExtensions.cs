using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

/// <summary>Provides the <c>ProfileSetupExtensions</c> API.</summary>
public static class ProfileSetupExtensions
{
    /// <summary>Provides the <c>SetupDefaultProfile</c> API.</summary>
    public static IProfile SetupDefaultProfile(this IProfileSetup setup, bool persist = true) => setup.SetupProfile(ProfileManager.DefaultProfile, persist);
}