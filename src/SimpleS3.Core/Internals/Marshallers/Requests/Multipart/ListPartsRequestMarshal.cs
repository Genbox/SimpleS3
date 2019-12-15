using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart
{
    [UsedImplicitly]
    internal class ListPartsRequestMarshal : IRequestMarshal<ListPartsRequest>
    {
        public Stream MarshalRequest(ListPartsRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(AmzParameters.MaxParts, request.MaxParts);
            request.AddQueryParameter(AmzParameters.PartNumberMarker, request.PartNumberMarker);
            return null;
        }
    }
}