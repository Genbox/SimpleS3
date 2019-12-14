using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets
{
    public class GetBucketTaggingResponse : BaseResponse
    {
        public IDictionary<string, string> Tags { get; internal set; }
    }
}