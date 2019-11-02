using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Multipart
{
    [UsedImplicitly]
    internal class ListMultipartUploadsRequestMarshal : IRequestMarshal<ListMultipartUploadsRequest>
    {
        public Stream MarshalRequest(ListMultipartUploadsRequest request, IS3Config config)
        {
            request.AddQueryParameter(BucketParameters.Delimiter, request.Delimiter);
            request.AddQueryParameter(BucketParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(BucketParameters.MaxUploads, request.MaxUploads);
            request.AddQueryParameter(BucketParameters.KeyMarker, request.KeyMarker);
            request.AddQueryParameter(BucketParameters.Prefix, request.Prefix);
            request.AddQueryParameter(BucketParameters.UploadIdMarker, request.UploadIdMarker);
            request.AddQueryParameter("uploads", string.Empty);
            return null;
        }
    }
}