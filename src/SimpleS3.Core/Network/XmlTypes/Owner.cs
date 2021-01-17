using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    public class Owner
    {
        public string DisplayName { get; set; }

        [XmlElement("ID")]
        public string Id { get; set; }
    }
}