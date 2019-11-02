using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Multipart
{
    [UsedImplicitly]
    internal class AbortMultipartUploadResponseMarshal : IResponseMarshal<AbortMultipartUploadRequest, AbortMultipartUploadResponse>
    {
        public void MarshalResponse(IS3Config config, AbortMultipartUploadRequest request, AbortMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
        }
    }
}