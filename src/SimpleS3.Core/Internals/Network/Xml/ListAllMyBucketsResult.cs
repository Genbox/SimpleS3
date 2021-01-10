using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class ListAllMyBucketsResult
    {
        [XmlElement]
        public Owner? Owner { get; set; }

        [XmlArrayItem("Bucket", IsNullable = false)]
        public List<Bucket>? Buckets { get; set; }
    }
}