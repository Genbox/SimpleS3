using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.Wasabi;

public class WasabiRegionData : IRegionData
{
    public static WasabiRegionData Instance { get; } = new WasabiRegionData();

    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield return new RegionInfo(WasabiRegion.UsEast1, "us-east-1", "North America (North Virginia)");
        yield return new RegionInfo(WasabiRegion.UsEast2, "us-east-2", "North America (North Virginia)");
        yield return new RegionInfo(WasabiRegion.UsCentral1, "us-central-1", "North America (Texas)");
        yield return new RegionInfo(WasabiRegion.UsWest1, "us-west-1", "North America (Oregon)");
        yield return new RegionInfo(WasabiRegion.CaCentral1, "ca-central-1", "Canada (Central)");
        yield return new RegionInfo(WasabiRegion.EuCentral1, "eu-central-1", "Europe (Amsterdam)");
        yield return new RegionInfo(WasabiRegion.EuCentral2, "eu-central-2", "Europe (Frankfurt)");
        yield return new RegionInfo(WasabiRegion.EuWest1, "eu-west-1", "Europe (London)");
        yield return new RegionInfo(WasabiRegion.EuWest2, "eu-west-2", "Europe (Paris)");
        yield return new RegionInfo(WasabiRegion.EuSouth1, "eu-south-1", "Europe (Milan)");
        yield return new RegionInfo(WasabiRegion.ApNorthEast1, "ap-northeast-1", "Asia Pacific (Tokyo)");
        yield return new RegionInfo(WasabiRegion.ApNorthEast2, "ap-northeast-2", "Asia Pacific (Osaka)");
        yield return new RegionInfo(WasabiRegion.ApSouthEast1, "ap-southeast-1", "Asia Pacific (Singapore)");
        yield return new RegionInfo(WasabiRegion.ApSouthEast2, "ap-southeast-2", "Asia Pacific (Sydney)");
    }
}