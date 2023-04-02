using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Core.Common.Extensions;

public static class RegionConverterExtensions
{
    public static IRegionInfo GetRegion(this IRegionConverter converter, Enum enumValue)
    {
        int intVal = (int)Convert.ChangeType(enumValue, typeof(int), NumberFormatInfo.InvariantInfo);
        return converter.GetRegion(intVal);
    }
}