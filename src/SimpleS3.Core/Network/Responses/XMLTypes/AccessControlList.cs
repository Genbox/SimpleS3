using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    public class AccessControlList
    {
        [XmlElement("Grant")]
        public List<Grant> Grants { get; set; }
    }
}