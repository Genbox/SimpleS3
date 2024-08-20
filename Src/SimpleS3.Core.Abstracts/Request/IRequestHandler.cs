using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface IRequestHandler
{
    Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken token = default) where TReq : IRequest where TResp : IResponse, new();
}