using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class GetBucketLifecycleConfigurationRequestMarshal : IRequestMarshal<GetBucketLifecycleConfigurationRequest>
    {
        public Stream? MarshalRequest(GetBucketLifecycleConfigurationRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Lifecycle, string.Empty);
            return null;
        }
    }
}