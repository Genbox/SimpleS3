using System.Security.Cryptography;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Provides the <c>CryptoHelper</c> API.</summary>
public static class CryptoHelper
{
    [ThreadStatic]private static HashAlgorithm? _sha256Cache;

    [ThreadStatic]private static HashAlgorithm? _md5Cache;

    private static HashAlgorithm Sha256 => _sha256Cache ??= SHA256.Create();
    private static HashAlgorithm Md5 => _md5Cache ??= MD5.Create();

    /// <summary>Provides the <c>Sha256Hash</c> API.</summary>
    public static byte[] Sha256Hash(byte[] data) => Sha256.ComputeHash(data);

    /// <summary>Provides the <c>Sha256Hash</c> API.</summary>
    public static byte[] Sha256Hash(byte[] data, int offset, int length) => Sha256.ComputeHash(data, offset, length);

    /// <summary>Provides the <c>Sha256Hash</c> API.</summary>
    public static byte[] Sha256Hash(Stream data, bool restorePosition)
    {
        long position = 0;

        if (restorePosition)
            position = data.Position;

        byte[] hash = Sha256.ComputeHash(data);

        if (restorePosition)
            data.Position = position;

        return hash;
    }

    /// <summary>Provides the <c>Md5Hash</c> API.</summary>
    public static byte[] Md5Hash(byte[] data) => Md5.ComputeHash(data);

    /// <summary>Provides the <c>Md5Hash</c> API.</summary>
    public static byte[] Md5Hash(Stream data, bool restorePosition)
    {
        long position = 0;

        if (restorePosition)
            position = data.Position;

        byte[] hash = Md5.ComputeHash(data);

        if (restorePosition)
            data.Position = position;

        return hash;
    }

    /// <summary>Provides the <c>HmacSign</c> API.</summary>
    public static byte[] HmacSign(byte[] data, byte[] key)
    {
        using KeyedHashAlgorithm algorithm = new HMACSHA256(key);
        return algorithm.ComputeHash(data);
    }
}