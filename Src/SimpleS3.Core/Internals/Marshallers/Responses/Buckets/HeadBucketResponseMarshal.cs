using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class HeadBucketResponseMarshal : IResponseMarshal<HeadBucketResponse>
{
    public void MarshalResponse(SimpleS3Config config, HeadBucketResponse response, IDictionary<string, string> headers, ContentStream responseStream) => response.BucketRegion = headers.GetOptionalValue("x-amz-bucket-region");
}