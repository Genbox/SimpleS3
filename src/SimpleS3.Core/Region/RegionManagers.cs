using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Region
{
    public static class RegionManagers
    {
        public static IRegionManager BuildAws()
        {
            RegionManager regionManager = new RegionManager();
            regionManager.Add((int)AwsRegion.ApEast1, "ap-east-1", "Asia Pacific (Hong Kong)");
            regionManager.Add((int)AwsRegion.ApNorthEast1, "ap-northeast-1", "Asia Pacific (Tokyo)");
            regionManager.Add((int)AwsRegion.ApNorthEast2, "ap-northeast-2", "Asia Pacific (Seoul)");
            regionManager.Add((int)AwsRegion.ApNorthEast3, "ap-northeast-3", "Asia Pacific (Osaka-Local)");
            regionManager.Add((int)AwsRegion.ApSouth1, "ap-south-1", "Asia Pacific (Mumbai)");
            regionManager.Add((int)AwsRegion.ApSouthEast1, "ap-southeast-1", "Asia Pacific (Singapore)");
            regionManager.Add((int)AwsRegion.ApSouthEast2, "ap-southeast-2", "Asia Pacific (Sydney)");
            regionManager.Add((int)AwsRegion.CaCentral1, "ca-central-1", "Canada (Central)");
            regionManager.Add((int)AwsRegion.CnNorth1, "cn-north-1", "China (Beijing)");
            regionManager.Add((int)AwsRegion.CnNorthWest1, "cn-northwest-1", "China (Ningxia)");
            regionManager.Add((int)AwsRegion.EuCentral1, "eu-central-1", "EU (Frankfurt)");
            regionManager.Add((int)AwsRegion.EuNorth1, "eu-north-1", "EU (Stockholm)");
            regionManager.Add((int)AwsRegion.EuWest1, "eu-west-1", "EU (Ireland)");
            regionManager.Add((int)AwsRegion.EuWest2, "eu-west-2", "EU (London)");
            regionManager.Add((int)AwsRegion.EuWest3, "eu-west-3", "EU (Paris)");
            regionManager.Add((int)AwsRegion.SaEast1, "sa-east-1", "South America (São Paulo)");
            regionManager.Add((int)AwsRegion.UsEast1, "us-east-1", "US East (N. Virginia)");
            regionManager.Add((int)AwsRegion.UsEast2, "us-east-2", "US East (Ohio)");
            regionManager.Add((int)AwsRegion.UsWest1, "us-west-1", "US West (N. California)");
            regionManager.Add((int)AwsRegion.UsWest2, "us-west-2", "US West (Oregon)");
            regionManager.Add((int)AwsRegion.MeSouth1, "me-south-1", "Middle East (Bahrain)");
            return regionManager;
        }
    }
}