using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal class Owner
    {
        public string DisplayName { get; set; }

        [XmlElement("ID")]
        public string Id { get; set; }
    }
}