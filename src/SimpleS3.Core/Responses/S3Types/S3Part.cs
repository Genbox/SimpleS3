using System;

namespace Genbox.SimpleS3.Core.Responses.S3Types
{
    public class S3Part
    {
        public int PartNumber { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ETag { get; set; }
        public long Size { get; set; }

        public override string ToString()
        {
            return $"Part: {PartNumber}";
        }
    }
}