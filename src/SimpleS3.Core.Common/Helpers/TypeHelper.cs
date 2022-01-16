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
                else if (type.IsAssignableFromGeneric(exportedType))
                    yield return exportedType;
            }
        }

        private static bool IsAssignableFromGeneric(this Type type, Type exportedType)
        {
            if (exportedType.IsGenericType && exportedType.GetGenericTypeDefinition() == type)
                return true;

            Type[] interfaceTypes = exportedType.GetInterfaces();

            foreach (Type ifaceType in interfaceTypes)
            {
                if (ifaceType.IsGenericType && ifaceType.GetGenericTypeDefinition() == type)
                    return true;
            }

            return false;
        }
    }
}