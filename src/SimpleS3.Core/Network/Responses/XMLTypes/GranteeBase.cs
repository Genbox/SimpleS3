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
}