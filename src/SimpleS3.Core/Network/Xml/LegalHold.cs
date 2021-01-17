using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Network.Xml
{
    [XmlRoot]
    public sealed class LegalHold
    {
        [XmlElement]
        public string Status { get; set; }
    }
}