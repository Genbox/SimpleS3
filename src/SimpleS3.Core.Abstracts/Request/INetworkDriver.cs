using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts.Request
{
    public interface INetworkDriver
    {
        Task<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)> SendRequestAsync(HttpMethod method, string url, IReadOnlyDictionary<string, string>? headers = null, Stream? dataStream = null, CancellationToken cancellationToken = default);
    }
}