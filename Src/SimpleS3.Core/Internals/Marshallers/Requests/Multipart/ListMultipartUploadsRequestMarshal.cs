using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;

internal sealed class ListMultipartUploadsRequestMarshal : IRequestMarshal<ListMultipartUploadsRequest>
{
    public Stream? MarshalRequest(ListMultipartUploadsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Uploads, string.Empty);
        request.SetOptionalQueryParameter(AmzParameters.Delimiter, request.Delimiter);

        if (request.EncodingType != EncodingType.Unknown)
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType.GetDisplayName());

        request.SetQueryParameter(AmzParameters.MaxUploads, request.MaxUploads);
        request.SetOptionalQueryParameter(AmzParameters.KeyMarker, request.KeyMarker);
        request.SetOptionalQueryParameter(AmzParameters.Prefix, request.Prefix);
        request.SetOptionalQueryParameter(AmzParameters.UploadIdMarker, request.UploadIdMarker);
        return null;
    }
}