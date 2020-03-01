using System;
using System.Collections.Generic;
using System.Linq;

namespace Genbox.SimpleS3.Core.Common
{
    /// <summary>
    /// A very simple service provider
    /// </summary>
    public class SimpleServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _obj;

        public SimpleServiceProvider(params Tuple<Type, object>[] obj)
        {
            _obj = obj.ToDictionary(x => x.Item1, y => y.Item2);
        }

        public object GetService(Type serviceType)
        {
            if (_obj.TryGetValue(serviceType, out object value))
                return value;

            return null;
        }
    }
}
