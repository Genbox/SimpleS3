using System;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Responses.S3Types
{
    public class S3Upload
    {
        public string ObjectKey { get; set; }
        public string UploadId { get; set; }
        public S3Identity Initiator { get; set; }
        public S3Identity Owner { get; set; }
        public StorageClass StorageClass { get; set; }
        public DateTimeOffset Initiated { get; set; }

        public override string ToString()
        {
            return $"{ObjectKey} ({UploadId})";
        }
    }
}