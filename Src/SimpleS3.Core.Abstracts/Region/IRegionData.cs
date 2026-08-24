namespace Genbox.SimpleS3.Core.Abstracts.Region;

/// <summary>Provides the <c>IRegionData</c> API.</summary>
public interface IRegionData
{
    /// <summary>Provides the <c>GetRegions</c> API.</summary>
    IEnumerable<IRegionInfo> GetRegions();
}