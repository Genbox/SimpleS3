using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class GetBucketLockConfigurationRequestMarshal : IRequestMarshal<GetBucketLockConfigurationRequest>
    {
        public Stream? MarshalRequest(GetBucketLockConfigurationRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.ObjectLock, string.Empty);
            return null;
        }
    }
}