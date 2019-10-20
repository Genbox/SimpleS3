using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class DeleteBucketRequestMarshal : IRequestMarshal<DeleteBucketRequest>
    {
        public Stream MarshalRequest(DeleteBucketRequest request, IS3Config config)
        {
            return null;
        }
    }
}