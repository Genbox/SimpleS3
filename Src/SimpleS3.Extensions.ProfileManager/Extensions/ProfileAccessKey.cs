using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public class ProfileAccessKey(IProfile profile) : IAccessKey
{
    public string KeyId { get; } = profile.KeyId;
    public byte[] SecretKey { get; } = profile.AccessKey;
}