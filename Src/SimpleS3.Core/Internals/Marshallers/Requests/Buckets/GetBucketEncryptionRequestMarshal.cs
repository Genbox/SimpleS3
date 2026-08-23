using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketEncryptionRequestMarshal : IRequestMarshal<GetBucketEncryptionRequest>
{
    public Stream? MarshalRequest(GetBucketEncryptionRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Encryption, string.Empty);
        return null;
    }
}