﻿using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.XmlTypes
{
    [UsedImplicitly]
    [XmlType]
    public sealed class CommonPrefix
    {
        public string Prefix { get; set; }
    }
}