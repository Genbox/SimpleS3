using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IRegionManager
    {
        IRegionInfo GetRegionInfo(int value);
        IEnumerable<IRegionInfo> GetAllRegions();
        IRegionInfo GetRegionInfo(string code);
    }
}