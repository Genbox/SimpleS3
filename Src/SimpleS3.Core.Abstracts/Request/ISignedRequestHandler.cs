using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface ISignedRequestHandler
{
    string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest;

    Task<TResp> SendRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new();
}