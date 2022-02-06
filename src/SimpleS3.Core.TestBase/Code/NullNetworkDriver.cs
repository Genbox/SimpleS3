using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.TestBase.Code;

public class NullNetworkDriver : INetworkDriver
{
    public string LastUrl { get; set; }

    public Task<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)> SendRequestAsync(HttpMethodType method, string url, IReadOnlyDictionary<string, string>? headers = null, Stream? dataStream = null, CancellationToken cancellationToken = default)
    {
        LastUrl = url;
        return Task.FromResult<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)>((200, new Dictionary<string, string>(), null));
    }
}