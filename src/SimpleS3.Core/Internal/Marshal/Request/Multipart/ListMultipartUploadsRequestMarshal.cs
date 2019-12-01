using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Multipart
{
    [UsedImplicitly]
    internal class ListMultipartUploadsRequestMarshal : IRequestMarshal<ListMultipartUploadsRequest>
    {
        public Stream MarshalRequest(ListMultipartUploadsRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.Uploads, string.Empty);
            request.AddQueryParameter(AmzParameters.Delimiter, request.Delimiter);
            request.AddQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(AmzParameters.MaxUploads, request.MaxUploads);
            request.AddQueryParameter(AmzParameters.KeyMarker, request.KeyMarker);
            request.AddQueryParameter(AmzParameters.Prefix, request.Prefix);
            request.AddQueryParameter(AmzParameters.UploadIdMarker, request.UploadIdMarker);
            return null;
        }
    }
}