using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.AwsS3
{
    public class AwsRegionData : IRegionData
    {
        public IEnumerable<IRegionInfo> GetRegions()
        {
            yield return new RegionInfo(AwsRegion.ApEast1, "ap-east-1", "Asia Pacific (Hong Kong)");
            yield return new RegionInfo(AwsRegion.ApNorthEast1, "ap-northeast-1", "Asia Pacific (Tokyo)");
            yield return new RegionInfo(AwsRegion.ApNorthEast2, "ap-northeast-2", "Asia Pacific (Seoul)");
            yield return new RegionInfo(AwsRegion.ApNorthEast3, "ap-northeast-3", "Asia Pacific (Osaka-Local)");
            yield return new RegionInfo(AwsRegion.ApSouth1, "ap-south-1", "Asia Pacific (Mumbai)");
            yield return new RegionInfo(AwsRegion.ApSouthEast1, "ap-southeast-1", "Asia Pacific (Singapore)");
            yield return new RegionInfo(AwsRegion.ApSouthEast2, "ap-southeast-2", "Asia Pacific (Sydney)");
            yield return new RegionInfo(AwsRegion.CaCentral1, "ca-central-1", "Canada (Central)");
            yield return new RegionInfo(AwsRegion.CnNorth1, "cn-north-1", "China (Beijing)");
            yield return new RegionInfo(AwsRegion.CnNorthWest1, "cn-northwest-1", "China (Ningxia)");
            yield return new RegionInfo(AwsRegion.EuCentral1, "eu-central-1", "EU (Frankfurt)");
            yield return new RegionInfo(AwsRegion.EuNorth1, "eu-north-1", "EU (Stockholm)");
            yield return new RegionInfo(AwsRegion.EuWest1, "eu-west-1", "EU (Ireland)");
            yield return new RegionInfo(AwsRegion.EuWest2, "eu-west-2", "EU (London)");
            yield return new RegionInfo(AwsRegion.EuWest3, "eu-west-3", "EU (Paris)");
            yield return new RegionInfo(AwsRegion.SaEast1, "sa-east-1", "South America (São Paulo)");
            yield return new RegionInfo(AwsRegion.UsEast1, "us-east-1", "US East (N. Virginia)");
            yield return new RegionInfo(AwsRegion.UsEast2, "us-east-2", "US East (Ohio)");
            yield return new RegionInfo(AwsRegion.UsWest1, "us-west-1", "US West (N. California)");
            yield return new RegionInfo(AwsRegion.UsWest2, "us-west-2", "US West (Oregon)");
            yield return new RegionInfo(AwsRegion.MeSouth1, "me-south-1", "Middle East (Bahrain)");
        }
    }
}