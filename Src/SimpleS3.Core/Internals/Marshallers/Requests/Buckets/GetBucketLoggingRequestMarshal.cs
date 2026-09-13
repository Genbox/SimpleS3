using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketLoggingRequestMarshal : IRequestMarshal<GetBucketLoggingRequest>
{
    public Stream? MarshalRequest(GetBucketLoggingRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("logging", string.Empty);
        return null;
    }
}