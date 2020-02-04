using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Genbox.SimpleS3.Core.Internals.Extensions
{
    internal static class StreamExtensions
    {
        public static async Task<int> ReadUpToAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken token = default)
        {
            int totalRead = 0;

            while (count > 0)
            {
                int numRead = await stream.ReadAsync(buffer, offset, count, token).ConfigureAwait(false);

                if (numRead == 0)
                    break;

                offset += numRead;
                count -= numRead;
                totalRead += numRead;
            }

            return totalRead;
        }
    }
}