using System;
using Genbox.SimpleS3.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Authentication
{
    public abstract class CredentialProviderBase : ICredentialProvider
    {
        private readonly byte[] _key;

        protected CredentialProviderBase(byte[] key)
        {
            _key = key;
        }

        public byte[] GetKey()
        {
            byte[] keyCopy = new byte[_key.Length];
            Array.Copy(_key, 0, keyCopy, 0, _key.Length);
            return keyCopy;
        }
    }
}