using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Xml
{
    internal class XmlSerializerCache
    {
        private readonly Dictionary<Type, XmlSerializer> _cache = new Dictionary<Type, XmlSerializer>();

        public XmlSerializer GetSerializerFor<T>()
        {
            Type type = typeof(T);

            if (!_cache.TryGetValue(type, out XmlSerializer serializer))
                _cache[type] = serializer = new XmlSerializer(type);

            return serializer;
        }
    }
}