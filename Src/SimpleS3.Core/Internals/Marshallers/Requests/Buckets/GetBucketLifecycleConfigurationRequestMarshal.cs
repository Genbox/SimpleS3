using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class GetBucketLifecycleConfigurationRequestMarshal : IRequestMarshal<GetBucketLifecycleConfigurationRequest>
{
    public Stream? MarshalRequest(GetBucketLifecycleConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Lifecycle, string.Empty);
        return null;
    }
}