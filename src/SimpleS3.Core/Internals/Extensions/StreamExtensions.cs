using System.IO;

namespace Genbox.SimpleS3.Core.Internals.Extensions
{
    internal static class StreamExtensions
    {
        public static int ReadUpTo(this Stream stream, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            while (count > 0)
            {
                int numRead = stream.Read(buffer, offset, count);

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