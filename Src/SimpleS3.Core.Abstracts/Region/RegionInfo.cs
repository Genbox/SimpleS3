namespace Genbox.SimpleS3.Core.Abstracts.Region;

public class RegionInfo(Enum enumValue, string code, string name) : IRegionInfo
{
    public Enum EnumValue { get; } = enumValue;
    public string Code { get; } = code;
    public string Name { get; } = name;
}