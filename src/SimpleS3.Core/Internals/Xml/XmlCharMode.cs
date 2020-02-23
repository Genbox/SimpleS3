using System;

namespace Genbox.SimpleS3.Core.Internals.Xml
{
    internal enum XmlCharMode
    {
        Unknown = 0,
        EntityEncode,
        Omit,
        ThrowException
    }
}