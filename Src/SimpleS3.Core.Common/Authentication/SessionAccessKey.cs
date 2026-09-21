using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Common.Authentication;

/// <summary>Stores temporary AWS credentials using the supplied secret byte array without copying it.</summary>
public class SessionAccessKey(string keyId, byte[] secretKey, string sessionToken) : AccessKeyBase(keyId, secretKey), ISessionAccessKey
{
    /// <summary>The AWS session token.</summary>
    public string SessionToken { get; } = sessionToken;
}