using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts
{
    public interface IProfileManager
    {
        IProfile GetProfile(string name = ProfileManager.DefaultProfile);
        string SaveProfile(IProfile profile);
        IProfile CreateProfile(string name, string keyId, byte[] accessKey, AwsRegion region, bool persist = true);
    }
}