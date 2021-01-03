using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class GetBucketVersioningRequestMarshal : IRequestMarshal<GetBucketVersioningRequest>
    {
        public Stream? MarshalRequest(GetBucketVersioningRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Versioning, string.Empty);

            return null;
        }
    }
}