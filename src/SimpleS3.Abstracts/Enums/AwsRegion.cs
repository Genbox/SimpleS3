using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Abstracts.Enums
{
    /// <summary>
    /// The AWS regions. See https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/Concepts.RegionsAndAvailabilityZones.html and
    /// https://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region
    /// </summary>
    public enum AwsRegion
    {
        Unknown = 0,

        /// <summary>Asia Pacific (Hong Kong)</summary>
        [EnumMember(Value = "ap-east-1")]
        APEast1,

        /// <summary>Asia Pacific (Tokyo)</summary>
        [EnumMember(Value = "ap-northeast-1")]
        APNorthEast1,

        /// <summary>Asia Pacific (Seoul)</summary>
        [EnumMember(Value = "ap-northeast-2")]
        APNorthEast2,

        /// <summary>Asia Pacific (Osaka-Local)</summary>
        [EnumMember(Value = "ap-northeast-3")]
        APNorthEast3,

        /// <summary>Asia Pacific (Mumbai)</summary>
        [EnumMember(Value = "ap-south-1")]
        APSouth1,

        /// <summary>Asia Pacific (Singapore)</summary>
        [EnumMember(Value = "ap-southeast-1")]
        APSouthEast1,

        /// <summary>Asia Pacific (Sydney)</summary>
        [EnumMember(Value = "ap-southeast-2")]
        APSouthEast2,

        /// <summary>Canada (Central)</summary>
        [EnumMember(Value = "ca-central-1")]
        CACentral1,

        /// <summary>China (Beijing)</summary>
        [EnumMember(Value = "cn-north-1")]
        CNNorth1,

        /// <summary>China (Ningxia)</summary>
        [EnumMember(Value = "cn-northwest-1")]
        CNNorthWest1,

        /// <summary>EU (Frankfurt)</summary>
        [EnumMember(Value = "eu-central-1")]
        EUCentral1,

        /// <summary>EU (Stockholm)</summary>
        [EnumMember(Value = "eu-north-1")]
        EUNorth1,

        /// <summary>EU (Ireland)</summary>
        [EnumMember(Value = "eu-west-1")]
        EUWest1,

        /// <summary>EU (London)</summary>
        [EnumMember(Value = "eu-west-2")]
        EUWest2,

        /// <summary>EU (Paris)</summary>
        [EnumMember(Value = "eu-west-3")]
        EUWest3,

        /// <summary>South America (Sao Paulo)</summary>
        [EnumMember(Value = "sa-east-1")]
        SAEast1,

        /// <summary>US East (N. Virginia)</summary>
        [EnumMember(Value = "us-east-1")]
        USEast1,

        /// <summary>US East (Ohio)</summary>
        [EnumMember(Value = "us-east-2")]
        USEast2,

        /// <summary>US West (N. California)</summary>
        [EnumMember(Value = "us-west-1")]
        USWest1,

        /// <summary>US West (Oregon)</summary>
        [EnumMember(Value = "us-west-2")]
        USWest2
    }
}