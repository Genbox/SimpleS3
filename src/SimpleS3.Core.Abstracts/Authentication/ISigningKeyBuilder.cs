using System;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication
{
    public interface ISigningKeyBuilder
    {
        byte[] CreateSigningKey(DateTimeOffset dateTime);
    }
}