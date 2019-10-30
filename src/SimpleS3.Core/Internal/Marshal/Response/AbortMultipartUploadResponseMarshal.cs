using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class AbortMultipartUploadResponseMarshal : IResponseMarshal<AbortMultipartUploadRequest, AbortMultipartUploadResponse>
    {
        public void MarshalResponse(AbortMultipartUploadRequest request, AbortMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
        }
    }
}