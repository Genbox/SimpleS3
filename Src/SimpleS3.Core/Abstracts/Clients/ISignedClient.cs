using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Clients;

/// <summary>Provides the <c>ISignedClient</c> API.</summary>
public interface ISignedClient
{
    /// <summary>Provides the <c>SignRequest&lt;TReq&gt;</c> API.</summary>
    string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest;
    /// <summary>Provides the <c>SendSignedRequestAsync&lt;TResp&gt;</c> API.</summary>
    Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new();
}