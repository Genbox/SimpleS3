using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketWebsiteRequestMarshal : IRequestMarshal<GetBucketWebsiteRequest>
{
    public Stream? MarshalRequest(GetBucketWebsiteRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Website, string.Empty);
        return null;
    }
}