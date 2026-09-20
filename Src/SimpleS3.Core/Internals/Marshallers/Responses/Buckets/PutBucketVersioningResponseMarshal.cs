using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class PutBucketVersioningResponseMarshal : IResponseMarshal<PutBucketVersioningResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutBucketVersioningResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        // PutBucketVersioning returns an empty body; common response fields are mapped by the handler.
    }
}