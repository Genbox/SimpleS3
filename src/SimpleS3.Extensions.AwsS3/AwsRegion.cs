namespace Genbox.SimpleS3.Extensions.AwsS3
{
    /// <summary>The AWS regions. See https://docs.aws.amazon.com/en_pv/general/latest/gr/rande.html#s3_region for more info</summary>
    public enum AwsRegion
    {
        /// <summary>Do not use this value</summary>
        Unknown = 0,

        /// <summary>Asia Pacific (Hong Kong)</summary>
        ApEast1,

        /// <summary>Asia Pacific (Tokyo)</summary>
        ApNorthEast1,

        /// <summary>Asia Pacific (Seoul)</summary>
        ApNorthEast2,

        /// <summary>Asia Pacific (Osaka-Local)</summary>
        ApNorthEast3,

        /// <summary>Asia Pacific (Mumbai)</summary>
        ApSouth1,

        /// <summary>Asia Pacific (Singapore)</summary>
        ApSouthEast1,

        /// <summary>Asia Pacific (Sydney)</summary>
        ApSouthEast2,

        /// <summary>Canada (Central)</summary>
        CaCentral1,

        /// <summary>China (Beijing)</summary>
        CnNorth1,

        /// <summary>China (Ningxia)</summary>
        CnNorthWest1,

        /// <summary>EU (Frankfurt)</summary>
        EuCentral1,

        /// <summary>EU (Stockholm)</summary>
        EuNorth1,

        /// <summary>EU (Ireland)</summary>
        EuWest1,

        /// <summary>EU (London)</summary>
        EuWest2,

        /// <summary>EU (Paris)</summary>
        EuWest3,

        /// <summary>South America (Sao Paulo)</summary>
        SaEast1,

        /// <summary>US East (N. Virginia)</summary>
        UsEast1,

        /// <summary>US East (Ohio)</summary>
        UsEast2,

        /// <summary>US West (N. California)</summary>
        UsWest1,

        /// <summary>US West (Oregon)</summary>
        UsWest2,

        /// <summary>Middle East (Bahrain)</summary>
        MeSouth1
    }
}