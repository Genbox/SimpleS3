using System.Text;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Authentication
{
    /// <summary>
    /// Provides a convenient way to provide an access key as string. However, it is insecure as you store the key as a string that does not get
    /// garbage collected.
    /// </summary>
    public class StringAccessKey : AccessKeyBase
    {
        public StringAccessKey(string keyId, string accessKey) : base(keyId, Encoding.UTF8.GetBytes(accessKey))
        {
            InputValidator.ValidateKeyId(keyId);
            InputValidator.ValidateAccessKey(Encoding.UTF8.GetBytes(accessKey));
        }
    }
}