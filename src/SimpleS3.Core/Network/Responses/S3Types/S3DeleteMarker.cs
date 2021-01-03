using System;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3DeleteMarker
    {
        public bool IsLatest { get; internal set; }
        public string ObjectKey { get; internal set; }
        public DateTimeOffset LastModified { get; internal set; }
        public Owner Owner { get; internal set; }
        public string VersionId { get; internal set; }
    }
}