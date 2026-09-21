using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Common.Authentication;

/// <summary>Creates temporary AWS credentials by encoding the supplied secret string into a new UTF-8 byte array.</summary>
public class StringSessionAccessKey(string keyId, string secretKey, string sessionToken) : AccessKeyBase(keyId, Encoding.UTF8.GetBytes(secretKey)), ISessionAccessKey
{
    /// <summary>The AWS session token.</summary>
    public string SessionToken { get; } = sessionToken;
}