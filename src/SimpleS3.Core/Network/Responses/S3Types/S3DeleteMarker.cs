using System;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3DeleteMarker
    {
        public S3DeleteMarker(bool isLatest, string objectKey, DateTimeOffset lastModified, Owner owner, string versionId)
        {
            IsLatest = isLatest;
            ObjectKey = objectKey;
            LastModified = lastModified;
            Owner = owner;
            VersionId = versionId;
        }

        public bool IsLatest { get; }
        public string ObjectKey { get; internal set; }
        public DateTimeOffset LastModified { get; }
        public Owner Owner { get; }
        public string VersionId { get; }
    }
}