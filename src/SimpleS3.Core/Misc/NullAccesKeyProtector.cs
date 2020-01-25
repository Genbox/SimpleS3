using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Misc
{
    public class NullAccesKeyProtector : IAccessKeyProtector
    {
        public byte[] ProtectKey(byte[] key)
        {
            return key;
        }

        public byte[] UnprotectKey(byte[] key)
        {
            return key;
        }
    }
}
