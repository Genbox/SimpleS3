using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Core.Common.Tests.Code;

public class TestRegionData : IRegionData
{
    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield return new RegionInfo(TestRegion.RegionOne, "Region-One", "The first region");
        yield return new RegionInfo(TestRegion.RegionTwo, "Region-Two", "The second region");
    }
}