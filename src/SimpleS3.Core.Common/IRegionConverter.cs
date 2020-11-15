using System;

namespace Genbox.SimpleS3.Core.Common
{
    public interface IRegionConverter
    {
        string GetRegion(Enum enumVal);
    }
}