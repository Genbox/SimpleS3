using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Region
{
    public class RegionInfo : IRegionInfo
    {
        public RegionInfo(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; }
        public string Name { get; }
    }
}