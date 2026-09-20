using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>INetworkDriver</c> API.</summary>
public interface INetworkDriver
{
    /// <summary>Provides the <c>SendRequestAsync&lt;T&gt;</c> API.</summary>
    [SuppressMessage("Design", "CA1054", Justification = "The signing pipeline preserves the raw escaped request URL; constructing a Uri earlier can normalize object keys and invalidate signatures.")]
    Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse;
}