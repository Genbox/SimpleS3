using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Responses.Buckets
{
    public class ListMultipartUploadsResponse : BaseResponse
    {
        public string Bucket { get; set; }
        public string KeyMarker { get; set; }
        public string UploadIdMarker { get; set; }
        public string NextKeyMarker { get; set; }
        public string NextUploadIdMarker { get; set; }
        public EncodingType EncodingType { get; set; }
        public int MaxUploads { get; set; }
        public bool IsTruncated { get; set; }
        public IList<S3Upload> Uploads { get; set; }
        public string Prefix { get; set; }
        public string Delimiter { get; set; }
        public IList<string> CommonPrefixes { get; set; }
    }
}