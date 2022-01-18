using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal class GetBucketAccelerateConfigurationRequestMarshal : IRequestMarshal<GetBucketAccelerateConfigurationRequest>
{
    public Stream? MarshalRequest(GetBucketAccelerateConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Accelerate, string.Empty);
        return null;
    }
}