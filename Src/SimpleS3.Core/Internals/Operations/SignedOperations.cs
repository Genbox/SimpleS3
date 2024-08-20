using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal class SignedOperations : ISignedOperations
{
    private readonly ISignedRequestHandler _signedHandler;

    public SignedOperations(ISignedRequestHandler signedHandler)
    {
        _signedHandler = signedHandler;
    }

    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest => _signedHandler.SignRequest(request, expiresIn);
    public Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new() => _signedHandler.SendRequestAsync<TResp>(url, httpMethod, content, token);
}