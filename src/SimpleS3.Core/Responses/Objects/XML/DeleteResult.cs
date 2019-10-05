using System.Collections.Generic;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Responses.XMLTypes;

namespace Genbox.SimpleS3.Core.Responses.Objects.XML
{
    [XmlRoot]
    public class DeleteResult
    {
        [XmlElement]
        public List<Deleted> Deleted { get; set; }

        [XmlElement]
        public List<Error> Error { get; set; }
    }
}