using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Provider;

public interface IEndpointBuilder
{
    IEndpointData GetEndpoint(IRequest request);
}