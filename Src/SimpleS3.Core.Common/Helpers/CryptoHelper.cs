using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Provides the <c>CryptoHelper</c> API.</summary>
[SuppressMessage("Design", "CA1062", Justification = "Stream and hash APIs already reject null input; these helpers preserve their existing exception behavior.")]
public static class CryptoHelper
{
    [ThreadStatic]private static HashAlgorithm? _sha256Cache;

    [ThreadStatic]private static HashAlgorithm? _md5Cache;

    private static HashAlgorithm Sha256 => _sha256Cache ??= SHA256.Create();

    [SuppressMessage("Security", "S4790", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
    [SuppressMessage("Security", "CA5351", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
    private static HashAlgorithm Md5 => _md5Cache ??= MD5.Create();

    /// <summary>Provides the <c>Sha256Hash</c> API.</summary>
    public static byte[] Sha256Hash(byte[] data) => Sha256.ComputeHash(data);

    /// <summary>Provides the <c>Sha256Hash</c> API.</summary>
    public static byte[] Sha256Hash(byte[] data, int offset, int length) => Sha256.ComputeHash(data, offset, length);

    /// <summary>Provides the <c>Sha256Hash</c> API.</summary>
    public static byte[] Sha256Hash(Stream data, bool restorePosition) => HashStream(Sha256, data, restorePosition);

    /// <summary>Provides the <c>Md5Hash</c> API.</summary>
    public static byte[] Md5Hash(byte[] data) => Md5.ComputeHash(data);

    /// <summary>Provides the <c>Md5Hash</c> API.</summary>
    public static byte[] Md5Hash(byte[] data, int offset, int length) => Md5.ComputeHash(data, offset, length);

    /// <summary>Provides the <c>Md5Hash</c> API.</summary>
    public static byte[] Md5Hash(Stream data, bool restorePosition) => HashStream(Md5, data, restorePosition);

    private static byte[] HashStream(HashAlgorithm algorithm, Stream data, bool restorePosition)
    {
        if (restorePosition && !data.CanSeek)
            throw new NotSupportedException("Restoring the position requires a seekable stream.");

        long position = restorePosition ? data.Position : 0;

        try
        {
            return algorithm.ComputeHash(data);
        }
        catch
        {
            // ComputeHash resets after success, but a failed read leaves partial input cached.
            algorithm.Initialize();
            throw;
        }
        finally
        {
            if (restorePosition)
                data.Position = position;
        }
    }

    /// <summary>Provides the <c>HmacSign</c> API.</summary>
    public static byte[] HmacSign(byte[] data, byte[] key)
    {
        using KeyedHashAlgorithm algorithm = new HMACSHA256(key);
        return algorithm.ComputeHash(data);
    }
}