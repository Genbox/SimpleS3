using System;
using Genbox.SimpleS3.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Authentication
{
    /// <summary>This class copies the key to ensure that when we clear the key material, we don't clear the original key.</summary>
    public class CopyAccessKeyProtector : IAccessKeyProtector
    {
        public byte[] ProtectKey(byte[] key)
        {
            return key;
        }

        public byte[] UnprotectKey(byte[] key)
        {
            byte[] keyCopy = new byte[key.Length];
            Array.Copy(key, 0, keyCopy, 0, key.Length);
            return keyCopy;
        }
    }
}