using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class ListPartsResult
    {
        public string Bucket { get; set; }
        public string? EncodingType { get; set; }
        public string Key { get; set; }
        public string UploadId { get; set; }
        public Initiator? Initiator { get; set; }
        public Owner? Owner { get; set; }
        public string? StorageClass { get; set; }
        public int PartNumberMarker { get; set; }
        public int NextPartNumberMarker { get; set; }
        public int MaxParts { get; set; }
        public bool IsTruncated { get; set; }

        [XmlElement("Part")]
        public List<Part>? Parts { get; set; }
    }
}