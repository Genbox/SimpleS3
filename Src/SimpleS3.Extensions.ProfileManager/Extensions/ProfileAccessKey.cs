using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

/// <summary>Provides the <c>ProfileAccessKey</c> API.</summary>
public class ProfileAccessKey(IProfile profile) : IAccessKey
{
    /// <summary>Provides the <c>KeyId</c> API.</summary>
    public string KeyId { get; } = profile.KeyId;
    /// <summary>Provides the <c>SecretKey</c> API.</summary>
    public byte[] SecretKey { get; } = profile.AccessKey;
}