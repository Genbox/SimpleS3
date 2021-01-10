using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    internal sealed class Bucket
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}