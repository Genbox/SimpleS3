namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

/// <summary>Provides the <c>IProfileSetup</c> API.</summary>
public interface IProfileSetup
{
    /// <summary>Provides the <c>SetupProfile</c> API.</summary>
    IProfile SetupProfile(string profileName, bool persist = true);
}