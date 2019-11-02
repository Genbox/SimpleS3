using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
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
            request.AddQueryParameter(AmzParameters.UploadId, request.UploadId);
            request.AddQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(AmzParameters.MaxParts, request.MaxParts);
            request.AddQueryParameter(AmzParameters.PartNumberMarker, request.PartNumberMarker);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);
            return null;
        }
    }
}