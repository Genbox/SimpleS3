using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IRegionData
    {
        IEnumerable<IRegionInfo> GetRegions();
    }
}