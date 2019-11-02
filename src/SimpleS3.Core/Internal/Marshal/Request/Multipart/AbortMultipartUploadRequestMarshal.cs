using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Multipart
{
    [UsedImplicitly]
    internal class AbortMultipartUploadRequestMarshal : IRequestMarshal<AbortMultipartUploadRequest>
    {
        public Stream MarshalRequest(AbortMultipartUploadRequest request, IS3Config config)
        {
            request.AddQueryParameter(MultipartParameters.UploadId, request.UploadId);
            return null;
        }
    }
}