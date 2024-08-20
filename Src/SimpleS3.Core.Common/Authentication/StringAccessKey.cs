using System.Text;

namespace Genbox.SimpleS3.Core.Common.Authentication;

/// <summary>Provides a convenient way to provide an access key as string. However, it is insecure as you store the key as a string that does not get garbage collected.</summary>
public class StringAccessKey(string keyId, string secretKey) : AccessKeyBase(keyId, Encoding.UTF8.GetBytes(secretKey));