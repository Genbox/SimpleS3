using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;

internal sealed class ListPartsRequestMarshal : IRequestMarshal<ListPartsRequest>
{
    public Stream? MarshalRequest(ListPartsRequest request, SimpleS3Config config)
    {
        if (request.EncodingType != EncodingType.Unknown)
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType.GetDisplayName());

        request.SetQueryParameter(AmzParameters.MaxParts, request.MaxParts);
        request.SetOptionalQueryParameter(AmzParameters.PartNumberMarker, request.PartNumberMarker);
        return null;
    }
}