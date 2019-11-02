using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    public sealed class Bucket
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}