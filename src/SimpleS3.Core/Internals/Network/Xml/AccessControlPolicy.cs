using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    internal sealed class AccessControlPolicy
    {
        [XmlElement]
        public Owner? Owner { get; set; }

        [XmlElement]
        public AccessControlList? AccessControlList { get; set; }
    }
}