using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal class SignedClient : ISignedClient
{
    private readonly ISignedOperations _operations;

    public SignedClient(ISignedOperations operations)
    {
        _operations = operations;
    }

    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest => _operations.SignRequest(request, expiresIn);

    public Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new() => _operations.SendSignedRequestAsync<TResp>(url, httpMethod, content, token);
}