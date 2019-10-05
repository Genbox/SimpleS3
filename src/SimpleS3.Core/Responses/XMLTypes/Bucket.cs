using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.XMLTypes
{
    [UsedImplicitly]
    [XmlType]
    public class Bucket
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}