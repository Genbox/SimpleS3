using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.GenericS3;

public class GenericS3RegionData : IRegionData
{
    public static GenericS3RegionData Instance { get; } = new GenericS3RegionData();

    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield break;
    }
}