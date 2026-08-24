namespace Genbox.SimpleS3.Core.Abstracts.Enums;

/// <summary>Provides the <c>SignatureMode</c> API.</summary>
public enum SignatureMode : byte
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Unsigned</c> API.</summary>
    Unsigned,
    /// <summary>Provides the <c>FullSignature</c> API.</summary>
    FullSignature,
    /// <summary>Provides the <c>StreamingSignature</c> API.</summary>
    StreamingSignature
}