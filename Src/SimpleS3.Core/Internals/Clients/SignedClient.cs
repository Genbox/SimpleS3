using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal sealed class SignedClient(ISignedRequestHandler handler) : ISignedClient
{
    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest => handler.SignRequest(request, expiresIn);

    public Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new() => handler.SendRequestAsync<TResp>(url, httpMethod, content, token);
}