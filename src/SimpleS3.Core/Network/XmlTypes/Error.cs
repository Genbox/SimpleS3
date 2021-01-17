using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    public sealed class Error
    {
        public string Key { get; set; }
        public string VersionId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}