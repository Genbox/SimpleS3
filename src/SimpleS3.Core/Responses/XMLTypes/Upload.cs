using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.XMLTypes
{
    [UsedImplicitly]
    [XmlType]
    public class Upload
    {
        public string Key { get; set; }
        public string UploadId { get; set; }
        public Initiator Initiator { get; set; }
        public Owner Owner { get; set; }
        public string StorageClass { get; set; }
        public DateTime Initiated { get; set; }
    }
}