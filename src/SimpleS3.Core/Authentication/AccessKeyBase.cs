using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Authentication
{
    /// <summary>Provides a common base for access keys</summary>
    public abstract class AccessKeyBase : IAccessKey
    {
        protected AccessKeyBase(string keyId, byte[] accessKey)
        {
            KeyId = keyId;
            AccessKey = accessKey;
        }

        public string KeyId { get; }
        public byte[] AccessKey { get; }
    }
}