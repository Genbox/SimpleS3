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
    internal class ListMultipartUploadsRequestMarshal : IRequestMarshal<ListMultipartUploadsRequest>
    {
        public Stream MarshalRequest(ListMultipartUploadsRequest request, IConfig config)
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