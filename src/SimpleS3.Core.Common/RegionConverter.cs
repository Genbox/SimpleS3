using System;
using System.Collections.Generic;
using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Core.Common
{
    public class RegionConverter : IRegionConverter
    {
        private readonly Dictionary<int, string> _enumMap = new Dictionary<int, string>(15);

        public RegionConverter(IRegionData data)
        {
            foreach (IRegionInfo regionInfo in data.GetRegions())
            {
                AddRegion(regionInfo);
            }
        }

        public string GetRegion(Enum enumVal)
        {
            int intVal = (int)Convert.ChangeType(enumVal, typeof(int), NumberFormatInfo.InvariantInfo);
            return _enumMap[intVal];
        }

        public void AddRegion(IRegionInfo regionInfo)
        {
            int intVal = (int)Convert.ChangeType(regionInfo.EnumValue, typeof(int), NumberFormatInfo.InvariantInfo);

            if (!_enumMap.ContainsKey(intVal))
                _enumMap.Add(intVal, regionInfo.Code);
        }
    }
}