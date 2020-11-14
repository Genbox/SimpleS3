using System;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public class RegionInfo : IRegionInfo
    {
        public RegionInfo(Enum enumValue, string code, string name)
        {
            EnumValue = enumValue;
            Code = code;
            Name = name;
        }

        public Enum EnumValue { get; }
        public string Code { get; }
        public string Name { get; }
    }
}