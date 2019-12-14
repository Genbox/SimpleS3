using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Bucket
{
    [UsedImplicitly]
    internal class DeleteBucketTaggingRequestMarshal : IRequestMarshal<DeleteBucketTaggingRequest>
    {
        public Stream MarshalRequest(DeleteBucketTaggingRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.Tagging, string.Empty);
            return null;
        }
    }
}