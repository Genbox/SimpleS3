namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts
{
    public interface IProfileManager
    {
        IProfile? GetProfile(string name);
        string SaveProfile(IProfile profile);
        IProfile CreateProfile(string name, string keyId, byte[] accessKey, string region, bool persist = true);
    }
}