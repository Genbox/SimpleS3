using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class ListPartsRequestMarshal : IRequestMarshal<ListPartsRequest>
    {
        public Stream MarshalRequest(ListPartsRequest request, IS3Config config)
        {
            request.AddQueryParameter(ObjectParameters.UploadId, request.UploadId);
            request.AddQueryParameter(ObjectParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(ObjectParameters.MaxParts, request.MaxParts);
            request.AddQueryParameter(ObjectParameters.PartNumberMarker, request.PartNumberMarker);
            return null;
        }
    }
}