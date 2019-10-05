using System;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Responses.S3Types
{
    public class S3Object
    {
        public string Name { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ETag { get; set; }
        public long Size { get; set; }
        public StorageClass StorageClass { get; set; }
        public S3ObjectIdentity Owner { get; set; }
    }
}