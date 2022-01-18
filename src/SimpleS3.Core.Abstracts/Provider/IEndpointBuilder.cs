using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Network;

namespace Genbox.SimpleS3.Core.Abstracts.Provider
{
    public interface IEndpointBuilder
    {
        IEndpointData GetEndpoint(IRequest request);
    }
}