using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class AbortMultipartUploadResponseMarshal : IResponseMarshal<AbortMultipartUploadResponse>
    {
        public void MarshalResponse(Config config, AbortMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}