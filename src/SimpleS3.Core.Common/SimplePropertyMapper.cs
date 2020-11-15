using System;
using System.Reflection;

namespace Genbox.SimpleS3.Core.Common
{
    public static class SimplePropertyMapper
    {
        public static void Map<T>(T from, T to) where T : class
        {
            Type toType = to.GetType();

            foreach (PropertyInfo propertyInfo in toType.GetProperties())
            {
                propertyInfo.SetValue(to, propertyInfo.GetValue(from));
            }
        }
    }
}