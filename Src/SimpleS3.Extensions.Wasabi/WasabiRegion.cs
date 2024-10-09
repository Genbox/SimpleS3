namespace Genbox.SimpleS3.Extensions.Wasabi;

/// <summary>The AWS regions. See https://docs.wasabi.com/docs/what-are-the-service-urls-for-wasabi-s-different-storage-regions for more info</summary>
public enum WasabiRegion
{
    /// <summary>Do not use this value</summary>
    Unknown = 0,

    /// <summary>North America (North Virginia)</summary>
    UsEast1,

    /// <summary>North America (North Virginia)</summary>
    UsEast2,

    /// <summary>North America (Texas)</summary>
    UsCentral1,

    /// <summary>North America (Oregon)</summary>
    UsWest1,

    /// <summary>Canada (Central)</summary>
    CaCentral1,

    /// <summary>Europe (Amsterdam)</summary>
    EuCentral1,

    /// <summary>Europe (Frankfurt)</summary>
    EuCentral2,

    /// <summary>Europe (London)</summary>
    EuWest1,

    /// <summary>Europe (Paris)</summary>
    EuWest2,

    /// <summary>Europe (Milan)</summary>
    EuSouth1,

    /// <summary>Asia Pacific (Tokyo)</summary>
    ApNorthEast1,

    /// <summary>Asia Pacific (Osaka)</summary>
    ApNorthEast2,

    /// <summary>Asia Pacific (Singapore)</summary>
    ApSouthEast1,

    /// <summary>Asia Pacific (Sydney)</summary>
    ApSouthEast2
}