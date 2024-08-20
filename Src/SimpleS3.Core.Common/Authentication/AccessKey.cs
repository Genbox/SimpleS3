namespace Genbox.SimpleS3.Core.Common.Authentication;

/// <summary>Stores an access key in raw format. This is more secure than using <see cref="StringAccessKey" /> as the backing byte array can be garbage collected.</summary>
public class AccessKey(string keyId, byte[] secretKey) : AccessKeyBase(keyId, secretKey);