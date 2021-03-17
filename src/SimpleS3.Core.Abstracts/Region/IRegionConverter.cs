namespace Genbox.SimpleS3.Core.Abstracts.Region
{
    public interface IRegionConverter
    {
        IRegionInfo GetRegion(int enumValue);
        IRegionInfo GetRegion(string regionCode);
    }
}