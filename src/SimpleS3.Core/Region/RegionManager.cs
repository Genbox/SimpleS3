using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Region
{
    public class RegionManager : IRegionManager
    {
        private readonly Dictionary<string, RegionInfo> _lookup = new Dictionary<string, RegionInfo>(StringComparer.Ordinal);
        private readonly Dictionary<int, RegionInfo> _enumToRegion = new Dictionary<int, RegionInfo>();

        public void Add(int index, string code, string name)
        {
            code = code.ToLowerInvariant();

            RegionInfo info = new RegionInfo(code, name);
            _lookup.Add(code, info);
            _enumToRegion.Add(index, info);
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
            foreach (KeyValuePair<string, RegionInfo> pair in _lookup)
            {
                yield return pair.Value;
            }
        }
    }
}