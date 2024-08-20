using System.Reflection;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class PropertyHelper
{
    public static void MapObjects<T, T2>(T? source, T2? destination) where T : class, T2 where T2 : class, new()
    {
        if (source == null || destination == null)
            return;

        Type sourceType = source.GetType();
        PropertyInfo[] sourceProperties = sourceType.GetProperties();

        Type destType = destination.GetType();
        PropertyInfo[] destProperties = destType.GetProperties();

        T2 defaultDestObj = new T2();

        int diff = sourceProperties.Length - destProperties.Length;

        for (int i = 0; i < destProperties.Length; i++)
        {
            PropertyInfo prop = sourceProperties[i + diff];

            object? sourceVal = prop.GetValue(source, null);
            object? destVal = prop.GetValue(destination, null);

            if (!IsDefaultValue(prop, destVal, defaultDestObj))
                continue;

            prop.SetValue(destination, sourceVal, null);
        }
    }

    private static bool IsDefaultValue(PropertyInfo propInfo, object? value, object defaultObj)
    {
        object? val = propInfo.GetValue(defaultObj, null);

        if (value == null && val == null)
            return true;

        if (value == null || val == null)
            return false;

        if (value.Equals(val))
            return true;

        return false;
    }
}