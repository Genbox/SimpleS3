using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

public class AmazonS3RegionData : IRegionData
{
    public static AmazonS3RegionData Instance { get; } = new AmazonS3RegionData();

    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield return new RegionInfo(AmazonS3Region.ApEast1, "ap-east-1", "Asia Pacific (Hong Kong)");
        yield return new RegionInfo(AmazonS3Region.ApNorthEast1, "ap-northeast-1", "Asia Pacific (Tokyo)");
        yield return new RegionInfo(AmazonS3Region.ApNorthEast2, "ap-northeast-2", "Asia Pacific (Seoul)");
        yield return new RegionInfo(AmazonS3Region.ApNorthEast3, "ap-northeast-3", "Asia Pacific (Osaka-Local)");
        yield return new RegionInfo(AmazonS3Region.ApSouth1, "ap-south-1", "Asia Pacific (Mumbai)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast1, "ap-southeast-1", "Asia Pacific (Singapore)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast2, "ap-southeast-2", "Asia Pacific (Sydney)");
        yield return new RegionInfo(AmazonS3Region.CaCentral1, "ca-central-1", "Canada (Central)");
        yield return new RegionInfo(AmazonS3Region.CnNorth1, "cn-north-1", "China (Beijing)");
        yield return new RegionInfo(AmazonS3Region.CnNorthWest1, "cn-northwest-1", "China (Ningxia)");
        yield return new RegionInfo(AmazonS3Region.EuCentral1, "eu-central-1", "EU (Frankfurt)");
        yield return new RegionInfo(AmazonS3Region.EuNorth1, "eu-north-1", "EU (Stockholm)");
        yield return new RegionInfo(AmazonS3Region.EuWest1, "eu-west-1", "EU (Ireland)");
        yield return new RegionInfo(AmazonS3Region.EuWest2, "eu-west-2", "EU (London)");
        yield return new RegionInfo(AmazonS3Region.EuWest3, "eu-west-3", "EU (Paris)");
        yield return new RegionInfo(AmazonS3Region.SaEast1, "sa-east-1", "South America (São Paulo)");
        yield return new RegionInfo(AmazonS3Region.UsEast1, "us-east-1", "US East (N. Virginia)");
        yield return new RegionInfo(AmazonS3Region.UsEast2, "us-east-2", "US East (Ohio)");
        yield return new RegionInfo(AmazonS3Region.UsWest1, "us-west-1", "US West (N. California)");
        yield return new RegionInfo(AmazonS3Region.UsWest2, "us-west-2", "US West (Oregon)");
        yield return new RegionInfo(AmazonS3Region.MeSouth1, "me-south-1", "Middle East (Bahrain)");
    }
}