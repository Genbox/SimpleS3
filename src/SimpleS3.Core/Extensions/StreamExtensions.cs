using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> AsDataAsync(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms).ConfigureAwait(false);

                return ms.ToArray();
            }
        }

        public static async Task<string> AsStringAsync(this Stream stream, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = Constants.Utf8NoBom;

            byte[] data = await AsDataAsync(stream).ConfigureAwait(false);
            return encoding.GetString(data);
        }

        public static async Task CopyToFileAsync(this Stream stream, string file)
        {
            string? dir = Path.GetDirectoryName(file);

            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (FileStream fs = File.OpenWrite(file))
                await stream.CopyToAsync(fs).ConfigureAwait(false);
        }
    }
}