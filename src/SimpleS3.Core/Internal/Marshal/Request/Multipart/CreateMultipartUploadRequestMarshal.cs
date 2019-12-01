using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Multipart
{
    [UsedImplicitly]
    internal class CreateMultipartUploadRequestMarshal : IRequestMarshal<CreateMultipartUploadRequest>
    {
        public Stream MarshalRequest(CreateMultipartUploadRequest request, IS3Config config)
        {
            //This is required for multipart uploads
            request.AddQueryParameter(AmzParameters.Uploads, string.Empty);
            return null;
        }
    }
}