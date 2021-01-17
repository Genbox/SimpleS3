using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Network.XmlTypes;

namespace Genbox.SimpleS3.Core.Network.Xml
{
    [XmlRoot]
    public sealed class ListMultipartUploadsResult
    {
        public string Bucket { get; set; }
        public string KeyMarker { get; set; }
        public string UploadIdMarker { get; set; }
        public string NextKeyMarker { get; set; }
        public string NextUploadIdMarker { get; set; }
        public string? EncodingType { get; set; }

        public int MaxUploads { get; set; }
        public bool IsTruncated { get; set; }

        [XmlElement("Upload")]
        public List<Upload>? Uploads { get; set; }

        public string Prefix { get; set; }
        public string Delimiter { get; set; }

        [XmlElement]
        public List<CommonPrefix>? CommonPrefixes { get; set; }
    }
}