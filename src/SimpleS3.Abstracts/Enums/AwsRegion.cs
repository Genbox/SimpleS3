using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Abstracts.Enums
{
    /// <summary>The AWS regions. See https://docs.aws.amazon.com/en_pv/general/latest/gr/rande.html#s3_region for more info</summary>
    public enum AwsRegion
    {
        /// <summary>Do not use this value</summary>
        Unknown = 0,

        /// <summary>Asia Pacific (Hong Kong)</summary>
        [EnumMember(Value = "ap-east-1")]
        ApEast1,

        /// <summary>Asia Pacific (Tokyo)</summary>
        [EnumMember(Value = "ap-northeast-1")]
        ApNorthEast1,

        /// <summary>Asia Pacific (Seoul)</summary>
        [EnumMember(Value = "ap-northeast-2")]
        ApNorthEast2,

        /// <summary>Asia Pacific (Osaka-Local)</summary>
        [EnumMember(Value = "ap-northeast-3")]
        ApNorthEast3,

        /// <summary>Asia Pacific (Mumbai)</summary>
        [EnumMember(Value = "ap-south-1")]
        ApSouth1,

        /// <summary>Asia Pacific (Singapore)</summary>
        [EnumMember(Value = "ap-southeast-1")]
        ApSouthEast1,

        /// <summary>Asia Pacific (Sydney)</summary>
        [EnumMember(Value = "ap-southeast-2")]
        ApSouthEast2,

        /// <summary>Canada (Central)</summary>
        [EnumMember(Value = "ca-central-1")]
        CaCentral1,

        /// <summary>China (Beijing)</summary>
        [EnumMember(Value = "cn-north-1")]
        CnNorth1,

        /// <summary>China (Ningxia)</summary>
        [EnumMember(Value = "cn-northwest-1")]
        CnNorthWest1,

        /// <summary>EU (Frankfurt)</summary>
        [EnumMember(Value = "eu-central-1")]
        EuCentral1,

        /// <summary>EU (Stockholm)</summary>
        [EnumMember(Value = "eu-north-1")]
        EuNorth1,

        /// <summary>EU (Ireland)</summary>
        [EnumMember(Value = "eu-west-1")]
        EuWest1,

        /// <summary>EU (London)</summary>
        [EnumMember(Value = "eu-west-2")]
        EuWest2,

        /// <summary>EU (Paris)</summary>
        [EnumMember(Value = "eu-west-3")]
        EuWest3,

        /// <summary>South America (Sao Paulo)</summary>
        [EnumMember(Value = "sa-east-1")]
        SaEast1,

        /// <summary>US East (N. Virginia)</summary>
        [EnumMember(Value = "us-east-1")]
        UsEast1,

        /// <summary>US East (Ohio)</summary>
        [EnumMember(Value = "us-east-2")]
        UsEast2,

        /// <summary>US West (N. California)</summary>
        [EnumMember(Value = "us-west-1")]
        UsWest1,

        /// <summary>US West (Oregon)</summary>
        [EnumMember(Value = "us-west-2")]
        UsWest2,

        /// <summary>Middle East (Bahrain)</summary>
        [EnumMember(Value = "me-south-1")]
        MeSouth1
    }
}