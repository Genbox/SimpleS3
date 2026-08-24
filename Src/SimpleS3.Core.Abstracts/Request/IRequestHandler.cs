using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>IRequestHandler</c> API.</summary>
public interface IRequestHandler
{
    /// <summary>Provides the <c>SendRequestAsync&lt;TReq,TResp&gt;</c> API.</summary>
    Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken token = default) where TReq : IRequest where TResp : IResponse, new();
}