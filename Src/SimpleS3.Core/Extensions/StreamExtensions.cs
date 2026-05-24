using System.Text;
using Genbox.SimpleS3.Core.Common.Misc;

namespace Genbox.SimpleS3.Core.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> AsDataAsync(this Stream stream, CancellationToken token = default)
    {
        using MemoryStream ms = new MemoryStream();
        await stream.CopyToAsync(ms, 81920, token).ConfigureAwait(false);

        return ms.ToArray();
    }

    public static async Task<string> AsStringAsync(this Stream stream, Encoding? encoding = null, CancellationToken token = default)
    {
        encoding ??= Constants.Utf8NoBom;

        byte[] data = await AsDataAsync(stream, token).ConfigureAwait(false);
        return encoding.GetString(data);
    }

    public static async Task CopyToFileAsync(this Stream stream, string file, CancellationToken token = default)
    {
        string? dir = Path.GetDirectoryName(file);

        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using FileStream fs = File.OpenWrite(file);
        await stream.CopyToAsync(fs, 81920, token).ConfigureAwait(false);
    }
}