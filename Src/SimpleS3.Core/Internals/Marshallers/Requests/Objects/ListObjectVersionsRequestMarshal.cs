using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class ListObjectVersionsRequestMarshal : IRequestMarshal<ListObjectVersionsRequest>
{
    public Stream? MarshalRequest(ListObjectVersionsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Versions, string.Empty);

        request.SetOptionalQueryParameter(AmzParameters.Delimiter, request.Delimiter);

        if (request.EncodingType != EncodingType.Unknown)
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType.GetDisplayName());

        request.SetOptionalQueryParameter(AmzParameters.KeyMarker, request.KeyMarker);
        request.SetQueryParameter(AmzParameters.MaxKeys, request.MaxKeys);
        request.SetOptionalQueryParameter(AmzParameters.Prefix, request.Prefix);
        request.SetOptionalQueryParameter(AmzParameters.VersionIdMarker, request.VersionIdMarker);

        return null;
    }
}