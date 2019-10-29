using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genbox.SimpleS3.Core.Misc
{
    public class ContentReader
    {
        private byte[] _data;

        public ContentReader(Stream inputStream)
        {
            InputStream = inputStream;
        }

        public Stream InputStream { get; set; }

        public Stream AsStream()
        {
            return InputStream;
        }

        public async Task<byte[]> AsDataAsync()
        {
            if (_data != null)
                return _data;

            using (MemoryStream ms = new MemoryStream())
            {
                await CopyToAsync(ms).ConfigureAwait(false);

                _data = ms.ToArray();
                return _data;
            }
        }

        public async Task<string> AsStringAsync(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            byte[] data = await AsDataAsync().ConfigureAwait(false);
            return encoding.GetString(data);
        }

        public async Task CopyToAsync(Stream stream, int bufferSize = 4096, CancellationToken token = default)
        {
            using (InputStream)
                await InputStream.CopyToAsync(stream, bufferSize, token).ConfigureAwait(false);
        }

        public async Task CopyToFileAsync(string file)
        {
            string dir = Path.GetDirectoryName(file);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (FileStream fs = File.OpenWrite(file))
                await CopyToAsync(fs).ConfigureAwait(false);
        }
    }
}