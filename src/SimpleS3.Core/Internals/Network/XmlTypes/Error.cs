using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal sealed class Error
    {
        public string Key { get; set; }
        public string VersionId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}