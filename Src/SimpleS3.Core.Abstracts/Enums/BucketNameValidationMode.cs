namespace Genbox.SimpleS3.Core.Abstracts.Enums;

public enum BucketNameValidationMode
{
    Unknown = 0,

    ///<summary>In this mode, only valid DNS characters is allowed</summary>
    DnsLabel,

    /// <summary>Bucket name validation is up to the provider. If you are not using a provider, it will default to
    /// <see cref="Unrestricted" /></summary>
    Default,

    /// <summary>Bucket name validation is up to the provider. In strict mode, a smaller set of characters (defined by the
    /// provider) is allowed for safety and compatibility. If you are not using a provider, it will default to
    /// <see cref="Unrestricted" /></summary>
    DefaultStrict,

    ///<summary>In this mode validation is still performed and only blacklisted characters are rejected</summary>
    Unrestricted,

    ///<summary>If you set validation to this flag, it completely disables validation</summary>
    Disabled
}