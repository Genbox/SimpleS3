using System;
using System.Reflection;

namespace Genbox.SimpleS3.Core.Common
{
    public static class PropertyMapper
    {
        public static void MapObjects<T, T2>(T source, T2 destination)
        {
            if (source == null || destination == null)
                return;

            Type sourceType = source.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();

            Type destType = destination.GetType();
            PropertyInfo[] destProperties = destType.GetProperties();

            foreach (PropertyInfo sourceProp in sourceProperties)
            {
                foreach (PropertyInfo destProp in destProperties)
                {
                    if (destProp.Name == sourceProp.Name)
                    {
                        destProp.SetValue(destination, sourceProp.GetValue(source, null), null);
                    }
                }
            }
        }
    }
}