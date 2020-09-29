using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Tests
{
    public class FakeNetworkDriver : INetworkDriver
    {
        public string SendResource { get; set; }

        public async Task<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)> SendRequestAsync(HttpMethod method, string url, IReadOnlyDictionary<string, string>? headers, Stream? dataStream, CancellationToken cancellationToken = default)
        {
            SendResource = url;
            return (200, new Dictionary<string, string>(), new MemoryStream(new byte[] { 1, 2, 3 }));
        }
    }
}