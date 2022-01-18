using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Abstracts.Region;

public interface IRegionData
{
    IEnumerable<IRegionInfo> GetRegions();
}