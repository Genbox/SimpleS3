using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Provider;

/// <summary>Provides the <c>IEndpointBuilder</c> API.</summary>
public interface IEndpointBuilder
{
    /// <summary>Provides the <c>GetEndpoint</c> API.</summary>
    IEndpointData GetEndpoint(IRequest request);
}