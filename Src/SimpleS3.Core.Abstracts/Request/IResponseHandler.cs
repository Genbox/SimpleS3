using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>IResponseHandler</c> API.</summary>
public interface IResponseHandler
{
    /// <summary>Provides the <c>HandleResponseAsync&lt;TReq,TResp&gt;</c> API.</summary>
    Task<TResp> HandleResponseAsync<TReq, TResp>(TReq request, string url, Stream? requestStream, CancellationToken token) where TReq : IRequest where TResp : IResponse, new();
}