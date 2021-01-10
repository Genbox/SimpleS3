using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [XmlType("CanonicalUser", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    internal class CanonicalUser : GranteeBase
    {
        [XmlElement("ID")]
        public string Id { get; set; }

        [XmlElement]
        public string DisplayName { get; set; }
    }
}