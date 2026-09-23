using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class ListBucketsRequestMarshal : IRequestMarshal<ListBucketsRequest>
{
    public Stream? MarshalRequest(ListBucketsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.MaxBuckets, request.MaxBuckets);
        request.SetOptionalQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
        request.SetOptionalQueryParameter(AmzParameters.Prefix, request.Prefix);
        request.SetOptionalQueryParameter(AmzParameters.BucketRegion, request.BucketRegion);
        return null;
    }
}