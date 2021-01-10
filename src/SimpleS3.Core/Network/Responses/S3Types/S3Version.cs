using System;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Version
    {
        public string Etag { get; internal set; }
        public bool IsLatest { get; internal set; }
        public string ObjectKey { get; internal set; }
        public DateTimeOffset LastModified { get; internal set; }
        public S3Identity Owner { get; internal set; }
        public int Size { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
        public string? VersionId { get; internal set; }
    }
}