using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketAnalyticsConfigurationRequestMarshal : IRequestMarshal<GetBucketAnalyticsConfigurationRequest>
{
    public Stream? MarshalRequest(GetBucketAnalyticsConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Analytics, string.Empty);
        request.SetQueryParameter(AmzParameters.Id, request.ConfigurationId);
        return null;
    }
}