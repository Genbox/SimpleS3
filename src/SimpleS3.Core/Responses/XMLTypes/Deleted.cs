using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.XMLTypes
{
    [UsedImplicitly]
    [XmlType]
    public class Deleted
    {
        public string Key { get; set; }
        public string VersionId { get; set; }
        public bool DeleteMarker { get; set; }
        public string DeleteMarkerVersionId { get; set; }
    }
}