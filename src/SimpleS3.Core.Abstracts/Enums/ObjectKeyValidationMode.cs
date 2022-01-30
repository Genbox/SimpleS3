namespace Genbox.SimpleS3.Core.Abstracts.Enums;

public enum ObjectKeyValidationMode
{
    Unknown = 0,

    ///<summary>Only safe characters such as a-z, A-Z and 0-9 are allowed</summary>
    SafeMode,

    ///<summary>Only the 7-bit ASCII code table is allowed</summary>
    AsciiMode,

    ///<summary>Only the 8-bit ASCII code table is allowed</summary>
    ExtendedAsciiMode,

    /// <summary>Provider specific validation will be performed in strict mode when using a provider. Strict mode checks that
    /// only safe and backwards compatible characters are used. When not using a provider, it will default to
    /// <see cref="Unrestricted" /></summary>
    DefaultStrict,

    /// <summary>Provider specific validation will be performed when using a provider. When not using a provider, it will
    /// default to <see cref="Unrestricted" /></summary>
    Default,

    ///<summary>In this mode validation is still performed and only blacklisted characters are rejected</summary>
    Unrestricted,

    ///<summary>If you set validation to this flag, it completely disables validation</summary>
    Disabled
}