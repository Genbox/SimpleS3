using System;
using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class CopyObjectResult
    {
        [XmlElement]
        public string ETag { get; set; }

        [XmlElement]
        public DateTime LastModified { get; set; }
    }
}