using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>IRequestMarshal</c> API.</summary>
[SuppressMessage("Design", "CA1040", Justification = "This marker interface identifies a capability or provides the non-generic base used by dependency injection and marshal discovery.")]
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IRequestMarshal;

/// <summary>Provides the <c>IRequestMarshal&lt;in T&gt;</c> API.</summary>
public interface IRequestMarshal<in T> : IRequestMarshal where T : IRequest
{
    /// <summary>Provides the <c>MarshalRequest</c> API.</summary>
    Stream? MarshalRequest(T request, SimpleS3Config config);
}