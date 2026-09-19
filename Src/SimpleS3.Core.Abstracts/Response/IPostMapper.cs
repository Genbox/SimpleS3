using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Request;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Response;

/// <summary>Provides the <c>IPostMapper</c> API.</summary>
[SuppressMessage("Design", "CA1040", Justification = "This marker interface identifies a capability or provides the non-generic base used by dependency injection and marshal discovery.")]
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IPostMapper;

/// <summary>Provides the <c>IPostMapper&lt;in TRequest,in TResponse&gt;</c> API.</summary>
public interface IPostMapper<in TRequest, in TResponse> : IPostMapper where TRequest : IRequest where TResponse : IResponse
{
    /// <summary>Provides the <c>PostMap</c> API.</summary>
    void PostMap(TRequest request, TResponse response);
}