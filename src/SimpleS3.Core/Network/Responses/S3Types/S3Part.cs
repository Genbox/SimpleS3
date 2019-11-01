using System;
using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Part : IHasETag
    {
        public int PartNumber { get; internal set; }
        public DateTimeOffset LastModified { get; internal set; }
        public long Size { get; internal set; }
        public string ETag { get; internal set; }

        public override string ToString()
        {
            return $"Part: {PartNumber}";
        }
    }
}