using System.Collections.Generic;
using Genbox.SimpleS3.Core.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Responses.Buckets
{
    public class ListBucketsResponse : BaseResponse
    {
        public S3Identity Owner { get; internal set; }
        public IList<S3Bucket> Buckets { get; internal set; }
    }
}