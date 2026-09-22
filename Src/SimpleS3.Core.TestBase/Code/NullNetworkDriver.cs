using System.Diagnostics.CodeAnalysis;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.TestBase.Code;

public class NullNetworkDriver : INetworkDriver
{
    [SuppressMessage("Design", "CA1056", Justification = "The captured request URL must preserve its exact escaped text for signing assertions.")]
    public string? LastUrl { get; private set; }

    public IReadOnlyDictionary<string, string>? LastHeaders { get; private set; }
    public HttpMethodType? LastMethod { get; private set; }
    public string? ResponseContent { get; set; }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don't ignore created IDisposable", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    public Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse
    {
        LastUrl = url;
        LastHeaders = new Dictionary<string, string>(request.Headers, StringComparer.OrdinalIgnoreCase);
        LastMethod = request.Method;
        ContentStream? content = null;

        if (ResponseContent != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(ResponseContent);
            content = new ContentStream(new MemoryStream(data, false), data.Length);
        }

        return Task.FromResult(new HttpResponse(content, new Dictionary<string, string>(StringComparer.Ordinal), 200));
    }
}