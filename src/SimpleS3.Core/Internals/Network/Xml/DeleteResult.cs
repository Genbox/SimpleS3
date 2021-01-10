using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class DeleteResult
    {
        [XmlElement]
        public List<Deleted>? Deleted { get; set; }

        [XmlElement]
        public List<Error>? Error { get; set; }
    }
}