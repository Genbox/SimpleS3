namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

public interface IProfileSetup
{
    IProfile SetupProfile(string profileName, bool persist = true);
}