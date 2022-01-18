using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public class GetBucketTaggingResponse : BaseResponse
{
    public GetBucketTaggingResponse()
    {
        Tags = new Dictionary<string, string>();
    }

    public IDictionary<string, string> Tags { get; }
}