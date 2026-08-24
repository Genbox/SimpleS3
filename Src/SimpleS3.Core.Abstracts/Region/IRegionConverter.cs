namespace Genbox.SimpleS3.Core.Abstracts.Region;

/// <summary>Provides the <c>IRegionConverter</c> API.</summary>
public interface IRegionConverter
{
    /// <summary>Provides the <c>GetRegion</c> API.</summary>
    IRegionInfo GetRegion(int enumValue);
    /// <summary>Provides the <c>GetRegion</c> API.</summary>
    IRegionInfo GetRegion(string regionCode);
}