using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart
{
    [UsedImplicitly]
    internal class ListPartsRequestMarshal : IRequestMarshal<ListPartsRequest>
    {
        public Stream? MarshalRequest(ListPartsRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.SetQueryParameter(AmzParameters.MaxParts, request.MaxParts);
            request.SetOptionalQueryParameter(AmzParameters.PartNumberMarker, request.PartNumberMarker);
            return null;
        }
    }
}