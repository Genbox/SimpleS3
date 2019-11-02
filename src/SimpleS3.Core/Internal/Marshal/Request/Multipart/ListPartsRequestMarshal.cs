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
    internal class ListPartsRequestMarshal : IRequestMarshal<ListPartsRequest>
    {
        public Stream MarshalRequest(ListPartsRequest request, IS3Config config)
        {
            request.AddQueryParameter(MultipartParameters.UploadId, request.UploadId);
            request.AddQueryParameter(ObjectParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(MultipartParameters.MaxParts, request.MaxParts);
            request.AddQueryParameter(MultipartParameters.PartNumberMarker, request.PartNumberMarker);
            return null;
        }
    }
}