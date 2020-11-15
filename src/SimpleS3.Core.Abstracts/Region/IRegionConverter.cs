using System;

namespace Genbox.SimpleS3.Core.Abstracts.Region
{
    public interface IRegionConverter
    {
        string GetRegion(Enum enumVal);
    }
}