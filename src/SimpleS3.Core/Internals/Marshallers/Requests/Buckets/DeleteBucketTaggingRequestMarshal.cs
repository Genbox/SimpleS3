using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal class DeleteBucketTaggingRequestMarshal : IRequestMarshal<DeleteBucketTaggingRequest>
{
    public Stream? MarshalRequest(DeleteBucketTaggingRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Tagging, string.Empty);
        return null;
    }
}