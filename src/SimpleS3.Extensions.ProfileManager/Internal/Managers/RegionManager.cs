using System;
using System.Collections.Generic;
using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal.Managers
{
    /// <summary>
    /// Converts between region codes and enum-as-integer values. This is used by the console setup system so that users can enter a numeric value
    /// for a region and it gets mapped to the correct value
    /// </summary>
    internal class RegionManager : IRegionManager
    {
        private readonly Dictionary<int, IRegionInfo> _enumToRegion = new Dictionary<int, IRegionInfo>();
        private readonly Dictionary<string, IRegionInfo> _lookup = new Dictionary<string, IRegionInfo>(StringComparer.Ordinal);

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