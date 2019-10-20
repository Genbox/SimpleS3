using System;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Abstracts.Extensions
{
    public static class AwsRegionExtensions
    {
        public static string GetRegionName(this AwsRegion region)
        {
            switch (region)
            {
                case AwsRegion.ApEast1:
                    return "Asia Pacific (Hong Kong)";
                case AwsRegion.ApNorthEast1:
                    return "Asia Pacific (Tokyo)";
                case AwsRegion.ApNorthEast2:
                    return "Asia Pacific (Seoul)";
                case AwsRegion.ApNorthEast3:
                    return "Asia Pacific (Osaka-Local)";
                case AwsRegion.ApSouth1:
                    return "Asia Pacific (Mumbai)";
                case AwsRegion.ApSouthEast1:
                    return "Asia Pacific (Singapore)";
                case AwsRegion.ApSouthEast2:
                    return "Asia Pacific (Sydney)";
                case AwsRegion.CaCentral1:
                    return "Canada (Central)";
                case AwsRegion.CnNorth1:
                    return "China (Beijing)";
                case AwsRegion.CnNorthWest1:
                    return "China (Ningxia)";
                case AwsRegion.EuCentral1:
                    return "EU (Frankfurt)";
                case AwsRegion.EuNorth1:
                    return "EU (Stockholm)";
                case AwsRegion.EuWest1:
                    return "EU (Ireland)";
                case AwsRegion.EuWest2:
                    return "EU (London)";
                case AwsRegion.EuWest3:
                    return "EU (Paris)";
                case AwsRegion.SaEast1:
                    return "South America (São Paulo)";
                case AwsRegion.UsEast1:
                    return "US East (N. Virginia)";
                case AwsRegion.UsEast2:
                    return "US East (Ohio)";
                case AwsRegion.UsWest1:
                    return "US West (N. California)";
                case AwsRegion.UsWest2:
                    return "US West (Oregon)";
                case AwsRegion.MeSouth1:
                    return "Middle East (Bahrain)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(region), region, null);
            }
        }

        public static string GetWorldSegment(this AwsRegion region)
        {
            switch (region)
            {
                case AwsRegion.ApEast1:
                case AwsRegion.ApNorthEast1:
                case AwsRegion.ApNorthEast2:
                case AwsRegion.ApNorthEast3:
                case AwsRegion.ApSouth1:
                case AwsRegion.ApSouthEast1:
                case AwsRegion.ApSouthEast2:
                    return "Asia Pacific";
                case AwsRegion.CaCentral1:
                    return "Canada";
                case AwsRegion.CnNorth1:
                case AwsRegion.CnNorthWest1:
                    return "China";
                case AwsRegion.EuCentral1:
                case AwsRegion.EuNorth1:
                case AwsRegion.EuWest1:
                case AwsRegion.EuWest2:
                case AwsRegion.EuWest3:
                    return "EU";
                case AwsRegion.SaEast1:
                    return "South America";
                case AwsRegion.UsEast1:
                case AwsRegion.UsEast2:
                case AwsRegion.UsWest1:
                case AwsRegion.UsWest2:
                    return "US";
                case AwsRegion.MeSouth1:
                    return "Middle East";
                default:
                    throw new ArgumentOutOfRangeException(nameof(region), region, null);
            }
        }
    }
}
