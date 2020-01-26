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
            request.SetQueryParameter(AmzParameters.Uploads, string.Empty);
            request.SetQueryParameter(AmzParameters.Delimiter, request.Delimiter);
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.SetQueryParameter(AmzParameters.MaxUploads, request.MaxUploads);
            request.SetQueryParameter(AmzParameters.KeyMarker, request.KeyMarker);
            request.SetQueryParameter(AmzParameters.Prefix, request.Prefix);
            request.SetQueryParameter(AmzParameters.UploadIdMarker, request.UploadIdMarker);
            return null;
        }
    }
}