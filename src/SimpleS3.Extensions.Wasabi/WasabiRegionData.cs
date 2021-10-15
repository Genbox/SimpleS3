using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.Wasabi
{
    public class WasabiRegionData : IRegionData
    {
        public static WasabiRegionData Instance { get; } = new WasabiRegionData();

        public IEnumerable<IRegionInfo> GetRegions()
        {
            yield return new RegionInfo(WasabiRegion.UsEast1, "us-east-1", "North America (North Virginia)");
            yield return new RegionInfo(WasabiRegion.UsEast2, "us-east-2", "North America (North Virginia)");
            yield return new RegionInfo(WasabiRegion.UsCentral1, "us-central-1", "North America (Texas)");
            yield return new RegionInfo(WasabiRegion.UsWest1, "us-west-1", "North America (Oregon)");
            yield return new RegionInfo(WasabiRegion.EuCentral1, "eu-central-1", "Europe (Amsterdam)");
            yield return new RegionInfo(WasabiRegion.EuWest1, "eu-west-1", "Europe (London)");
            yield return new RegionInfo(WasabiRegion.ApNorthEast1, "ap-northeast-1", "Asia Pacific (Tokyo)");
            yield return new RegionInfo(WasabiRegion.ApNorthEast2, "ap-northeast-2", "Asia Pacific (Osaka)");
        }
    }
}