using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketAbacRequestMarshal : IRequestMarshal<GetBucketAbacRequest>
{
    public Stream? MarshalRequest(GetBucketAbacRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("abac", string.Empty);
        return null;
    }
}