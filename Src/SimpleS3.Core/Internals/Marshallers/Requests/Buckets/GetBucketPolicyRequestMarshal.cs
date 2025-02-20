using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketPolicyRequestMarshal : IRequestMarshal<GetBucketPolicyRequest>
{
    public Stream? MarshalRequest(GetBucketPolicyRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Policy, string.Empty);
        return null;
    }
}