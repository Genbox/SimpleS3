using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface INetworkDriver
{
    Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse;
}