using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Core.Common;

/// <summary>Converts between region codes and enums.</summary>
public class RegionConverter : IRegionConverter
{
    private readonly Dictionary<int, IRegionInfo> _enumMap = new Dictionary<int, IRegionInfo>();
    private readonly Dictionary<string, IRegionInfo> _stringMap = new Dictionary<string, IRegionInfo>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Provides the <c>RegionConverter</c> API.</summary>
    public RegionConverter(IRegionData? data = null) //If we are not provided with region data, we don't do any mapping
    {
        if (data == null)
            return;

        foreach (IRegionInfo regionInfo in data.GetRegions())
        {
            int intVal = (int)Convert.ChangeType(regionInfo.EnumValue, typeof(int), NumberFormatInfo.InvariantInfo);

            _enumMap.Add(intVal, regionInfo);
            _stringMap.Add(regionInfo.Code, regionInfo);
        }
    }

    /// <summary>Provides the <c>GetRegion</c> API.</summary>
    public IRegionInfo GetRegion(int enumValue) => _enumMap[enumValue];

    /// <summary>Provides the <c>GetRegion</c> API.</summary>
    public IRegionInfo GetRegion(string regionCode) => _stringMap[regionCode];

    internal IRegionInfo? FindRegion(string? regionCode) => regionCode != null && _stringMap.TryGetValue(regionCode, out IRegionInfo? region) ? region : null;
}