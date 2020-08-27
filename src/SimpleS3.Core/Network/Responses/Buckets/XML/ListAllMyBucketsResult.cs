using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets.Xml
{
    [XmlRoot]
    public sealed class ListAllMyBucketsResult
    {
        [XmlElement]
        public Owner? Owner { get; set; }

        [XmlArrayItem("Bucket", IsNullable = false)]
        public List<Bucket>? Buckets { get; set; }
    }
}