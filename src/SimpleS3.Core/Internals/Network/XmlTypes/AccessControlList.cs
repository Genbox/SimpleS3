using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal class AccessControlList
    {
        [XmlElement("Grant")]
        public List<Grant> Grants { get; set; }
    }
}