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

        using MemoryStream buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, 81920, token);
        return encoding.GetString(buffer.GetBuffer(), 0, checked((int)buffer.Length));
    }

    /// <summary>Provides the <c>CopyToFileAsync</c> API.</summary>
    public static async Task CopyToFileAsync(this Stream stream, string file, CancellationToken token = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        token.ThrowIfCancellationRequested();

        string? dir = Path.GetDirectoryName(file);

        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // S3-006: Create truncates an existing destination so stale trailing bytes cannot remain.
        using FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fs, 81920, token);
    }
}