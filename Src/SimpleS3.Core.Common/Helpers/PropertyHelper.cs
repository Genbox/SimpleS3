using System.Reflection;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Provides the <c>PropertyHelper</c> API.</summary>
public static class PropertyHelper
{
    /// <summary>Provides the <c>MapObjects&lt;T,T2&gt;</c> API.</summary>
    public static void MapObjects<T, T2>(T? source, T2? destination) where T : class, T2 where T2 : class, new()
    {
        if (source == null || destination == null)
            return;

        PropertyInfo[] properties = typeof(T2).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        T2 defaultDestObj = new T2();

        foreach (PropertyInfo prop in properties)
        {
            if (!prop.CanRead || !prop.CanWrite || prop.GetIndexParameters().Length != 0)
                continue;

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