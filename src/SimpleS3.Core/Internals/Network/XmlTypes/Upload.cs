using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal sealed class Upload
    {
        public string Key { get; set; }
        public string UploadId { get; set; }
        public Initiator? Initiator { get; set; }
        public Owner? Owner { get; set; }
        public string? StorageClass { get; set; }
        public DateTime Initiated { get; set; }
    }
}