using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Responses.XMLTypes;

namespace Genbox.SimpleS3.Core.Responses.Objects.XML
{
    [XmlRoot]
    public class ListPartsResult
    {
        public string Bucket { get; set; }

        [XmlElement("Encoding-Type")]
        public string EncodingType { get; set; }

        public string Key { get; set; }
        public string UploadId { get; set; }
        public Initiator Initiator { get; set; }
        public Owner Owner { get; set; }
        public string StorageClass { get; set; }
        public int PartNumberMarker { get; set; }
        public int NextPartNumberMarker { get; set; }
        public int MaxParts { get; set; }
        public bool IsTruncated { get; set; }

        [XmlElement("Part")]
        public List<Part> Part { get; set; }
    }
}