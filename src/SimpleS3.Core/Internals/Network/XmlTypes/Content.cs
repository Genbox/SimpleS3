using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal sealed class Content
    {
        public string Key { get; set; }
        public DateTime LastModified { get; set; }
        public string ETag { get; set; }
        public long Size { get; set; }
        public string? StorageClass { get; set; }

        [XmlElement]
        public Owner? Owner { get; set; }
    }
}