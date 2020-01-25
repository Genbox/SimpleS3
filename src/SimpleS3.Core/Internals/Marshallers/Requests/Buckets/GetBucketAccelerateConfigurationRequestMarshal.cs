using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class GetBucketAccelerateConfigurationRequestMarshal : IRequestMarshal<GetBucketAccelerateConfigurationRequest>
    {
        public Stream MarshalRequest(GetBucketAccelerateConfigurationRequest request, IConfig config)
        {
            request.AddQueryParameter(AmzParameters.Accelerate, string.Empty);
            return null;
        }
    }
}