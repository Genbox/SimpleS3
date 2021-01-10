using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [XmlType("Group", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    internal class Group : GranteeBase
    {
        [XmlElement("URI")]
        public string Uri { get; set; }
    }
}