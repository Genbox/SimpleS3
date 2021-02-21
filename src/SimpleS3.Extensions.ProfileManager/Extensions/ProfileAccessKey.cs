using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions
{
    public class ProfileAccessKey : IAccessKey
    {
        public ProfileAccessKey(IProfile profile)
        {
            KeyId = profile.KeyId;
            SecretKey = profile.AccessKey;
        }

        public string KeyId { get; }
        public byte[] SecretKey { get; }
    }
}