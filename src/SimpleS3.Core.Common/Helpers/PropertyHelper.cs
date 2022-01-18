using System.Reflection;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class PropertyHelper
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
                    object? sourceVal = sourceProp.GetValue(source, null);

                    if (sourceVal == null)
                        continue;

                    destProp.SetValue(destination, sourceProp.GetValue(source, null), null);
                }
            }
        }
    }
}