using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>INetworkDriver</c> API.</summary>
public interface INetworkDriver
{
    /// <summary>Provides the <c>SendRequestAsync&lt;T&gt;</c> API.</summary>
    Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse;
}