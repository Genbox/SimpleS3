using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Common.Authentication;

/// <summary>Provides a common base for access keys</summary>
public abstract class AccessKeyBase(string keyId, byte[] secretKey) : IAccessKey
{
    public string KeyId { get; } = keyId;
    public byte[] SecretKey { get; } = secretKey;
}