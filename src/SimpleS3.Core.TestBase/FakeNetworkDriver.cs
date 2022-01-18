using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.TestBase;

public class FakeNetworkDriver : INetworkDriver
{
    public string SendResource { get; set; }

    public async Task<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)> SendRequestAsync(HttpMethodType method, string url, IReadOnlyDictionary<string, string>? headers = null, Stream? dataStream = null, CancellationToken cancellationToken = default)
    {
        SendResource = url;
        return (200, new Dictionary<string, string>(), new MemoryStream(new byte[] { 1, 2, 3 }));
    }
}