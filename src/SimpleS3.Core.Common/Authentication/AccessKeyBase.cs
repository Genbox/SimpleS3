using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Common.Authentication
{
    /// <summary>Provides a common base for access keys</summary>
    public abstract class AccessKeyBase : IAccessKey
    {
        protected AccessKeyBase(string keyId, byte[] secretKey)
        {
            KeyId = keyId;
            SecretKey = secretKey;
        }

        public string KeyId { get; }
        public byte[] SecretKey { get; }
    }
}