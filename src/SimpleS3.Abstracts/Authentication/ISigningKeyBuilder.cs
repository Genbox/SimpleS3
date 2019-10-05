using System;

namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface ISigningKeyBuilder
    {
        byte[] CreateSigningKey(DateTimeOffset dateTime, string service);
    }
}