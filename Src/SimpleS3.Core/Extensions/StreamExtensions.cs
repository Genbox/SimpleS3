using System.Text;
using Genbox.SimpleS3.Core.Common.Misc;

namespace Genbox.SimpleS3.Core.Extensions;

/// <summary>Provides the <c>StreamExtensions</c> API.</summary>
public static class StreamExtensions
{
    /// <summary>Provides the <c>AsDataAsync</c> API.</summary>
    public static async Task<byte[]> AsDataAsync(this Stream stream, CancellationToken token = default)
    {
        using MemoryStream ms = new MemoryStream();
        await stream.CopyToAsync(ms, 81920, token);

        return ms.ToArray();
    }

    /// <summary>Provides the <c>AsStringAsync</c> API.</summary>
    public static async Task<string> AsStringAsync(this Stream stream, Encoding? encoding = null, CancellationToken token = default)
    {
        encoding ??= Constants.Utf8NoBom;

        byte[] data = await stream.AsDataAsync(token);
        return encoding.GetString(data);
    }

    /// <summary>Provides the <c>CopyToFileAsync</c> API.</summary>
    public static async Task CopyToFileAsync(this Stream stream, string file, CancellationToken token = default)
    {
        string? dir = Path.GetDirectoryName(file);

        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // S3-006: Create truncates an existing destination so stale trailing bytes cannot remain.
        using FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fs, 81920, token);
    }
}