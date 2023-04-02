using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface IRequestHandler
{
    Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken token = default) where TResp : IResponse, new() where TReq : IRequest;
}