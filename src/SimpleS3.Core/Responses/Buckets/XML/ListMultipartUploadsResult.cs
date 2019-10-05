using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Responses.XMLTypes;

namespace Genbox.SimpleS3.Core.Responses.Buckets.XML
{
    [XmlRoot]
    public class ListMultipartUploadsResult
    {
        public string Bucket { get; set; }
        public string KeyMarker { get; set; }
        public string UploadIdMarker { get; set; }
        public string NextKeyMarker { get; set; }
        public string NextUploadIdMarker { get; set; }

        [XmlElement("Encoding-Type")]
        public string EncodingType { get; set; }

        public int MaxUploads { get; set; }
        public bool IsTruncated { get; set; }

        [XmlElement("Upload")]
        public List<Upload> Uploads { get; set; }

        public string Prefix { get; set; }
        public string Delimiter { get; set; }

        [XmlElement]
        public List<CommonPrefix> CommonPrefixes { get; set; }
    }
}