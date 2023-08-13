using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal class PutBucketVersioningResponseMarshal : IResponseMarshal<PutBucketVersioningResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutBucketVersioningResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        // The docs says there is an XML response, but there is none from the API.
    }
}