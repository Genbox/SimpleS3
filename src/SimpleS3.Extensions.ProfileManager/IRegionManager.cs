using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.ProfileManager
{
    public interface IRegionManager
    {
        IEnumerable<IRegionInfo> GetAllRegions();
        IRegionInfo GetRegionInfo(int value);
        IRegionInfo GetRegionInfo(string code);
    }
}