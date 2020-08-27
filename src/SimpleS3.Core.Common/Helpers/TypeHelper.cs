using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genbox.SimpleS3.Core.Common.Helpers
{
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetInstanceTypesInheritedFrom<T>(Assembly? assembly = null)
        {
            return GetInstanceTypesInheritedFrom(typeof(T), assembly);
        }

        public static IEnumerable<Type> GetInstanceTypesInheritedFrom(Type type, Assembly? assembly = null)
        {
            if (assembly == null)
                assembly = type.Assembly;

            foreach (Type exportedType in GetTypesInheritedFrom(type, assembly))
            {
                if (!exportedType.IsClass)
                    continue;

                if (exportedType.IsAbstract)
                    continue;

                if (exportedType.IsInterface)
                    continue;

                yield return exportedType;
            }
        }

        public static IEnumerable<Type> GetTypesInheritedFrom(Type type, Assembly? assembly = null)
        {
            if (assembly == null)
                assembly = type.Assembly;

            foreach (Type exportedType in assembly.GetTypes())
            {
                //Skip the type itself - we only want types that inherit from the type
                if (exportedType == type)
                    continue;

                if (type.IsAssignableFrom(exportedType))
                    yield return exportedType;
            }
        }
    }
}
