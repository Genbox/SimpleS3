using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.XmlTypes
{
    [UsedImplicitly]
    [XmlInclude(typeof(CanonicalUser))]
    [XmlInclude(typeof(Group))]
    [XmlType(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public abstract class GranteeBase
    {
    }

    [XmlType("CanonicalUser", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class CanonicalUser : GranteeBase
    {
        [XmlElement("ID")]
        public string Id { get; set; }

        [XmlElement]
        public string DisplayName { get; set; }
    }

    [XmlType("Group", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Group : GranteeBase
    {
        [XmlElement("URI")]
        public string Uri { get; set; }
    }
}