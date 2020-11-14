using System;
using System.Collections.Generic;
using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager
{
    public class RegionManager : IRegionManager
    {
        private readonly Dictionary<string, IRegionInfo> _lookup = new Dictionary<string, IRegionInfo>(StringComparer.Ordinal);
        private readonly Dictionary<int, IRegionInfo> _enumToRegion = new Dictionary<int, IRegionInfo>();

        public RegionManager(IRegionData regionData)
        {
            foreach (IRegionInfo region in regionData.GetRegions())
            {
                _lookup.Add(region.Code, region);
                _enumToRegion.Add((int)Convert.ChangeType(region.EnumValue, typeof(int), NumberFormatInfo.InvariantInfo), region);
            }
        }

        public IRegionInfo GetRegionInfo(string code)
        {
            return _lookup[code];
        }

        public IRegionInfo GetRegionInfo(int value)
        {
            return _enumToRegion[value];
        }

        public IEnumerable<IRegionInfo> GetAllRegions()
        {
            foreach (KeyValuePair<string, IRegionInfo> pair in _lookup)
            {
                yield return pair.Value;
            }
        }
    }
}