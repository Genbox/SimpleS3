using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class ListBucketInventoryConfigurationsRequestMarshal : IRequestMarshal<ListBucketInventoryConfigurationsRequest>
{
    public Stream? MarshalRequest(ListBucketInventoryConfigurationsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Inventory, string.Empty);
        if (request.ContinuationToken != null)
            request.SetQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
        return null;
    }
}