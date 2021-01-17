using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    public class Grant
    {
        [XmlElement]
        public List<GranteeBase> Grantee { get; set; }

        [XmlElement]
        public string Permission { get; set; }
    }
}