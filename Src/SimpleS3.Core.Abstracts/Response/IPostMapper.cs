using Genbox.SimpleS3.Core.Abstracts.Request;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Response;

/// <summary>Provides the <c>IPostMapper</c> API.</summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IPostMapper;

/// <summary>Provides the <c>IPostMapper&lt;in TRequest,in TResponse&gt;</c> API.</summary>
public interface IPostMapper<in TRequest, in TResponse> : IPostMapper where TRequest : IRequest where TResponse : IResponse
{
    /// <summary>Provides the <c>PostMap</c> API.</summary>
    void PostMap(TRequest request, TResponse response);
}