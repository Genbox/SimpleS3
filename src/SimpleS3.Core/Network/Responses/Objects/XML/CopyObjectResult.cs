using System;
using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects.Xml
{
    [XmlRoot]
    public sealed class CopyObjectResult
    {
        [XmlElement]
        public string ETag { get; set; }

        [XmlElement]
        public DateTime LastModified { get; set; }
    }
}