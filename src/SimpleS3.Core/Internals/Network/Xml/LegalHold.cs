using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class LegalHold
    {
        [XmlElement]
        public string Status { get; set; }
    }
}