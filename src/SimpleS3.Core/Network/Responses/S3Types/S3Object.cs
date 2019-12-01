using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Object : IHasStorageClass, IHasETag
    {
        public string ObjectKey { get; internal set; }
        public DateTimeOffset LastModifiedOn { get; internal set; }
        public long Size { get; internal set; }

        public S3Identity Owner { get; internal set; }
        public string ETag { get; internal set; }
        public StorageClass StorageClass { get; internal set; }

        public override string ToString()
        {
            return ObjectKey;
        }
    }
}