using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Operations;

/// <summary>Provides the <c>ISignedOperations</c> API.</summary>
public interface ISignedOperations
{
    /// <summary>Provides the <c>SignRequest&lt;TReq&gt;</c> API.</summary>
    string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest;
    /// <summary>Provides the <c>SendSignedRequestAsync&lt;TResp&gt;</c> API.</summary>
    Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new();
}