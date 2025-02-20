using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketPolicyRequestMarshal : IRequestMarshal<PutBucketPolicyRequest>
{
    public Stream MarshalRequest(PutBucketPolicyRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Policy, string.Empty);
        request.SetHeader(AmzHeaders.XAmzConfirmRemoveSelfBucketAccess, request.ConfirmRemoveSelfBucketAccess ? "true" : "false");

        byte[] bytes = Encoding.UTF8.GetBytes(request.Policy);
        return new MemoryStream(bytes);
    }
}