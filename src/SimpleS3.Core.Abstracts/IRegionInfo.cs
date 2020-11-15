using System;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IRegionInfo
    {
        Enum EnumValue { get; }
        string Code { get; }
        string Name { get; }
    }
}