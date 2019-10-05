using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class AbortMultipartUploadRequestMarshal : IRequestMarshal<AbortMultipartUploadRequest>
    {
        public Stream MarshalRequest(AbortMultipartUploadRequest request)
        {
            request.AddQueryParameter(ObjectParameters.UploadId, request.UploadId);
            return null;
        }
    }
}