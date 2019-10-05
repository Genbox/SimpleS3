using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Responses.XMLTypes;

namespace Genbox.SimpleS3.Core.Responses.Buckets.XML
{
    [XmlRoot]
    public class ListBucketResult
    {
        public string Name { get; set; }
        public string Prefix { get; set; }

        [XmlElement]
        public List<CommonPrefix> CommonPrefixes { get; set; }

        [XmlElement("Encoding-Type")]
        public string EncodingType { get; set; }

        public string ContinuationToken { get; set; }
        public string NextContinuationToken { get; set; }
        public string StartAfter { get; set; }
        public int KeyCount { get; set; }
        public int MaxKeys { get; set; }
        public bool IsTruncated { get; set; }

        [XmlElement]
        public List<Content> Contents { get; set; }
    }
}