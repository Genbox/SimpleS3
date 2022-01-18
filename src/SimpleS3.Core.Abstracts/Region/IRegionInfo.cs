namespace Genbox.SimpleS3.Core.Abstracts.Region;

public interface IRegionInfo
{
    Enum EnumValue { get; }
    string Code { get; }
    string Name { get; }
}