using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.TestBase.Code;

public class NullNetworkDriver : INetworkDriver
{
    [SuppressMessage("Design", "CA1056", Justification = "The captured request URL must preserve its exact escaped text for signing assertions.")]
    public string? LastUrl { get; private set; }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don't ignore created IDisposable", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    public Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse
    {
        LastUrl = url;
        return Task.FromResult(new HttpResponse(null, new Dictionary<string, string>(), 200));
    }
}