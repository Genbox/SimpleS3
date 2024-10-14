using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

public class AmazonS3RegionData : IRegionData
{
    public static AmazonS3RegionData Instance { get; } = new AmazonS3RegionData();

    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield return new RegionInfo(AmazonS3Region.AfSouth1, "af-south-1", "Africa (Cape Town)");
        yield return new RegionInfo(AmazonS3Region.ApEast1, "ap-east-1", "Asia Pacific (Hong Kong)");
        yield return new RegionInfo(AmazonS3Region.ApNorthEast1, "ap-northeast-1", "Asia Pacific (Tokyo)");
        yield return new RegionInfo(AmazonS3Region.ApNorthEast2, "ap-northeast-2", "Asia Pacific (Seoul)");
        yield return new RegionInfo(AmazonS3Region.ApNorthEast3, "ap-northeast-3", "Asia Pacific (Osaka-Local)");
        yield return new RegionInfo(AmazonS3Region.ApSouth1, "ap-south-1", "Asia Pacific (Mumbai)");
        yield return new RegionInfo(AmazonS3Region.ApSouth2, "ap-south-2", "Asia Pacific (Hyderabad)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast1, "ap-southeast-1", "Asia Pacific (Singapore)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast2, "ap-southeast-2", "Asia Pacific (Sydney)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast3, "ap-southeast-3", "Asia Pacific (Jakarta)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast4, "ap-southeast-4", "Asia Pacific (Melbourne)");
        yield return new RegionInfo(AmazonS3Region.ApSouthEast5, "ap-southeast-5", "Asia Pacific (Malaysia)");
        yield return new RegionInfo(AmazonS3Region.CaCentral1, "ca-central-1", "Canada (Central)");
        yield return new RegionInfo(AmazonS3Region.CaWest1, "ca-west-1", "Canada West (Calgary)");
        yield return new RegionInfo(AmazonS3Region.CnNorth1, "cn-north-1", "China (Beijing)");
        yield return new RegionInfo(AmazonS3Region.CnNorthWest1, "cn-northwest-1", "China (Ningxia)");
        yield return new RegionInfo(AmazonS3Region.EuCentral1, "eu-central-1", "EU (Frankfurt)");
        yield return new RegionInfo(AmazonS3Region.EuCentral2, "eu-central-2", "EU (Zurich)");
        yield return new RegionInfo(AmazonS3Region.EuNorth1, "eu-north-1", "EU (Stockholm)");
        yield return new RegionInfo(AmazonS3Region.EuSouth1, "eu-south-1", "EU (Milan)");
        yield return new RegionInfo(AmazonS3Region.EuSouth2, "eu-south-2", "EU (Spain)");
        yield return new RegionInfo(AmazonS3Region.EuWest1, "eu-west-1", "EU (Ireland)");
        yield return new RegionInfo(AmazonS3Region.EuWest2, "eu-west-2", "EU (London)");
        yield return new RegionInfo(AmazonS3Region.EuWest3, "eu-west-3", "EU (Paris)");
        yield return new RegionInfo(AmazonS3Region.IlCentral1, "il-central-1", "Israel (Tel Aviv)");
        yield return new RegionInfo(AmazonS3Region.MeCentral1, "me-central-1", "Middle East (UAE)");
        yield return new RegionInfo(AmazonS3Region.MeSouth1, "me-south-1", "Middle East (Bahrain)");
        yield return new RegionInfo(AmazonS3Region.SaEast1, "sa-east-1", "South America (São Paulo)");
        yield return new RegionInfo(AmazonS3Region.UsEast1, "us-east-1", "US East (N. Virginia)");
        yield return new RegionInfo(AmazonS3Region.UsEast2, "us-east-2", "US East (Ohio)");
        yield return new RegionInfo(AmazonS3Region.UsWest1, "us-west-1", "US West (N. California)");
        yield return new RegionInfo(AmazonS3Region.UsWest2, "us-west-2", "US West (Oregon)");
        yield return new RegionInfo(AmazonS3Region.UsGovEast1, "us-gov-east-1", "AWS GovCloud (US-East)");
        yield return new RegionInfo(AmazonS3Region.UsGovWest1, "us-gov-west-1", "AWS GovCloud (US-West)");
    }
}