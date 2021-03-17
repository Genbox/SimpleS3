using System;
using System.Collections.Generic;
using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Core.Common
{
    /// <summary>
    /// Converts between region codes and enums.
    /// </summary>
    public class RegionConverter : IRegionConverter
    {
        private readonly Dictionary<int, IRegionInfo> _enumMap = new Dictionary<int, IRegionInfo>();
        private readonly Dictionary<string, IRegionInfo> _stringMap = new Dictionary<string, IRegionInfo>(StringComparer.OrdinalIgnoreCase);

        public RegionConverter(IRegionData data)
        {
            foreach (IRegionInfo regionInfo in data.GetRegions())
            {
                int intVal = (int)Convert.ChangeType(regionInfo.EnumValue, typeof(int), NumberFormatInfo.InvariantInfo);

                _enumMap.Add(intVal, regionInfo);
                _stringMap.Add(regionInfo.Code, regionInfo);
            }
        }

        public IRegionInfo GetRegion(int enumValue)
        {
            return _enumMap[enumValue];
        }

        public IRegionInfo GetRegion(string regionCode)
        {
            return _stringMap[regionCode];
        }
    }
}