using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Responses.Buckets
{
    public class ListObjectsResponse : BaseResponse
    {
        public string Name { get; set; }

        public string Prefix { get; set; }

        public int KeyCount { get; set; }

        public int MaxKeys { get; set; }

        public bool IsTruncated { get; set; }

        public IList<string> CommonPrefixes { get; set; }

        public EncodingType EncodingType { get; set; }

        public string ContinuationToken { get; set; }

        public string NextContinuationToken { get; set; }

        public string StartAfter { get; set; }

        public IList<S3Object> Objects { get; set; }
    }
}