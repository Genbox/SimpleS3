using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>ISignedRequestHandler</c> API.</summary>
public interface ISignedRequestHandler
{
    /// <summary>Provides the <c>SignRequest&lt;TReq&gt;</c> API.</summary>
    string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest;

    /// <summary>Provides the <c>SendRequestAsync&lt;TResp&gt;</c> API.</summary>
    [SuppressMessage("Design", "CA1054", Justification = "The signing pipeline preserves the raw escaped request URL; constructing a Uri earlier can normalize object keys and invalidate signatures.")]
    Task<TResp> SendRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new();
}