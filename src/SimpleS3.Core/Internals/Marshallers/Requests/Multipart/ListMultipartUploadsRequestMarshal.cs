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
    internal class ListMultipartUploadsRequestMarshal : IRequestMarshal<ListMultipartUploadsRequest>
    {
        public Stream? MarshalRequest(ListMultipartUploadsRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Uploads, string.Empty);
            request.SetOptionalQueryParameter(AmzParameters.Delimiter, request.Delimiter);
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.SetQueryParameter(AmzParameters.MaxUploads, request.MaxUploads);
            request.SetOptionalQueryParameter(AmzParameters.KeyMarker, request.KeyMarker);
            request.SetOptionalQueryParameter(AmzParameters.Prefix, request.Prefix);
            request.SetOptionalQueryParameter(AmzParameters.UploadIdMarker, request.UploadIdMarker);
            return null;
        }
    }
}