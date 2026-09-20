using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Common.Authentication;

/// <summary>Provides a common base for access keys</summary>
public abstract class AccessKeyBase(string keyId, byte[] secretKey) : IAccessKey
{
    /// <summary>Provides the <c>KeyId</c> API.</summary>
    public string KeyId { get; } = keyId;

    /// <summary>Provides the <c>SecretKey</c> API.</summary>
    public byte[] SecretKey { get; } = secretKey;
}