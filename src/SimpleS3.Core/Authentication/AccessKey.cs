namespace Genbox.SimpleS3.Core.Authentication
{
    /// <summary>
    /// Stores an access key in raw format. This is more secure than using <see cref="StringAccessKey" /> as the backing byte array can be garbage
    /// collected.
    /// </summary>
    public class AccessKey : AccessKeyBase
    {
        public AccessKey(string keyId, byte[] accessKey) : base(keyId, accessKey) { }
    }
}