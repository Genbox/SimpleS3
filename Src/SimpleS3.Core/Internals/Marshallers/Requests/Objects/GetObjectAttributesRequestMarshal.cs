using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class GetObjectAttributesRequestMarshal : IRequestMarshal<GetObjectAttributesRequest>
{
    public Stream? MarshalRequest(GetObjectAttributesRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Attributes, string.Empty);
        request.SetHeader(AmzHeaders.XAmzObjectAttributes, string.Join(",", request.ObjectAttributes.Where(x => x != ObjectAttribute.Unknown).Select(x => x.GetDisplayName())));
        if (request.MaxParts.HasValue)
            request.SetHeader("x-amz-max-parts", ValueHelper.IntToString(request.MaxParts.Value));
        if (request.PartNumberMarker.HasValue)
            request.SetHeader("x-amz-part-number-marker", ValueHelper.IntToString(request.PartNumberMarker.Value));
        return null;
    }
}