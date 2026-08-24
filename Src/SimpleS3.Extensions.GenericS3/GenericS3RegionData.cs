using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.GenericS3;

/// <summary>Provides the <c>GenericS3RegionData</c> API.</summary>
public class GenericS3RegionData : IRegionData
{
    /// <summary>Provides the <c>Instance</c> API.</summary>
    public static GenericS3RegionData Instance { get; } = new GenericS3RegionData();

    /// <summary>Provides the <c>GetRegions</c> API.</summary>
    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield break;
    }
}