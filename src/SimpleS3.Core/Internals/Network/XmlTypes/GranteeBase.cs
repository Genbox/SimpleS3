using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlInclude(typeof(CanonicalUser))]
    [XmlInclude(typeof(Group))]
    [XmlType(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    internal abstract class GranteeBase { }
}