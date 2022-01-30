namespace Genbox.SimpleS3.Extensions.Wasabi;

/// <summary>The AWS regions. See
/// https://wasabi-support.zendesk.com/hc/en-us/articles/360015106031-What-are-the-service-URLs-for-Wasabi-s-different-regions-
/// for more info</summary>
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

    /// <summary>Europe (Amsterdam)</summary>
    EuCentral1,

    /// <summary>Europe (London)</summary>
    EuWest1,

    /// <summary>Asia Pacific (Tokyo)</summary>
    ApNorthEast1,

    /// <summary>Asia Pacific (Osaka)</summary>
    ApNorthEast2
}