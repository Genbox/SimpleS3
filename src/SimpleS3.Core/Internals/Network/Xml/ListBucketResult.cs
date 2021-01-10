using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class ListBucketResult
    {
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Delimiter { get; set; }

        [XmlElement]
        public List<CommonPrefix>? CommonPrefixes { get; set; }

        public string? EncodingType { get; set; }
        public string ContinuationToken { get; set; }
        public string NextContinuationToken { get; set; }
        public string StartAfter { get; set; }
        public int KeyCount { get; set; }
        public int MaxKeys { get; set; }
        public bool IsTruncated { get; set; }

        [XmlElement]
        public List<Content>? Contents { get; set; }
    }
}