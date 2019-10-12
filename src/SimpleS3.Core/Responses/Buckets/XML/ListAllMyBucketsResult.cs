using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Responses.XMLTypes;

namespace Genbox.SimpleS3.Core.Responses.Buckets.XML
{
    [XmlRoot]
    public class ListAllMyBucketsResult
    {
        [XmlElement]
        public Owner Owner { get; set; }

        [XmlArrayItem("Bucket", IsNullable = false)]
        public List<Bucket> Buckets { get; set; }
    }
}