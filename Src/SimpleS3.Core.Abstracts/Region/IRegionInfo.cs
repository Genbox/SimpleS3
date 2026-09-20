namespace Genbox.SimpleS3.Core.Abstracts.Region;

/// <summary>Provides the <c>IRegionInfo</c> API.</summary>
public interface IRegionInfo
{
    /// <summary>Provides the <c>EnumValue</c> API.</summary>
    Enum EnumValue { get; }

    /// <summary>Provides the <c>Code</c> API.</summary>
    string Code { get; }

    /// <summary>Provides the <c>Name</c> API.</summary>
    string Name { get; }
}