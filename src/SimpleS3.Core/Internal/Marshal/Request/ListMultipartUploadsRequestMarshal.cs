using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class ListMultipartUploadsRequestMarshal : IRequestMarshal<ListMultipartUploadsRequest>
    {
        public Stream MarshalRequest(ListMultipartUploadsRequest request, IS3Config config)
        {
            request.AddHeader(BucketParameters.Delimiter, request.Delimiter);
            request.AddHeader(BucketParameters.EncodingType, request.EncodingType);
            request.AddHeader(BucketParameters.MaxUploads, request.MaxUploads);
            request.AddHeader(BucketParameters.KeyMarker, request.KeyMarker);
            request.AddHeader(BucketParameters.Prefix, request.Prefix);
            request.AddHeader(BucketParameters.UploadIdMarker, request.UploadIdMarker);
            request.AddQueryParameter("uploads", string.Empty);
            return null;
        }
    }
}