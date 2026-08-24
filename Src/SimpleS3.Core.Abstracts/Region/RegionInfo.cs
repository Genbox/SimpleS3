namespace Genbox.SimpleS3.Core.Abstracts.Region;

/// <summary>Provides the <c>RegionInfo</c> API.</summary>
public class RegionInfo(Enum enumValue, string code, string name) : IRegionInfo
{
    /// <summary>Provides the <c>EnumValue</c> API.</summary>
    public Enum EnumValue { get; } = enumValue;
    /// <summary>Provides the <c>Code</c> API.</summary>
    public string Code { get; } = code;
    /// <summary>Provides the <c>Name</c> API.</summary>
    public string Name { get; } = name;
}