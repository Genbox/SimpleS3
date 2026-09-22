using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class ListBucketMetricsConfigurationsRequestMarshal : IRequestMarshal<ListBucketMetricsConfigurationsRequest>
{
    public Stream? MarshalRequest(ListBucketMetricsConfigurationsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Metrics, string.Empty);
        if (request.ContinuationToken != null)
            request.SetQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
        return null;
    }
}