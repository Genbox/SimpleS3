using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class ListObjectsRequestMarshal : IRequestMarshal<ListObjectsRequest>
{
    public Stream? MarshalRequest(ListObjectsRequest request, SimpleS3Config config)
    {
        request.SetOptionalQueryParameter(AmzParameters.Delimiter, request.Delimiter);

        if (request.EncodingType != EncodingType.Unknown)
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType.GetDisplayName());

        request.SetQueryParameter(AmzParameters.MaxKeys, request.MaxKeys);
        request.SetOptionalQueryParameter(AmzParameters.Prefix, request.Prefix);
        request.SetOptionalQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
        request.SetQueryParameter(AmzParameters.FetchOwner, request.FetchOwner);
        request.SetOptionalQueryParameter(AmzParameters.StartAfter, request.StartAfter);
        request.SetQueryParameter(AmzParameters.ListType, 2);
        return null;
    }
}