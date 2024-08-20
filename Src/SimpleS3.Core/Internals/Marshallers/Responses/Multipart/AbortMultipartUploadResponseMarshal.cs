using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;

internal sealed class AbortMultipartUploadResponseMarshal : IResponseMarshal<AbortMultipartUploadResponse>
{
    public void MarshalResponse(SimpleS3Config config, AbortMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
    }
}