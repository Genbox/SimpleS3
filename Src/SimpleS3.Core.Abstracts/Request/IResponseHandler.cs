using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface IResponseHandler
{
    Task<TResp> HandleResponseAsync<TReq, TResp>(TReq request, string url, Stream? requestStream, CancellationToken token) where TReq : IRequest where TResp : IResponse, new();
}