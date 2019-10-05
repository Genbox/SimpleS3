using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class ListPartsResponse : BaseResponse
    {
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string UploadId { get; set; }
        public StorageClass StorageClass { get; set; }
        public int PartNumberMarker { get; set; }
        public int NextPartNumberMarker { get; set; }
        public int MaxParts { get; set; }
        public bool IsTruncated { get; set; }
        public EncodingType EncodingType { get; set; }
        public S3ObjectIdentity Owner { get; set; }
        public S3ObjectIdentity Initiator { get; set; }
        public IList<S3Part> Parts { get; set; }
    }
}