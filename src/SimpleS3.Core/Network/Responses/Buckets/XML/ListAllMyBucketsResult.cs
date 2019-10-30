using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Network.Responses.XMLTypes;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets.XML
{
    [XmlRoot]
    public sealed class ListAllMyBucketsResult
    {
        [XmlElement]
        public Owner Owner { get; set; }

        [XmlArrayItem("Bucket", IsNullable = false)]
        public List<Bucket> Buckets { get; set; }
    }
}