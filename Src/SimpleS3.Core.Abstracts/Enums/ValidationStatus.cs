using System.Diagnostics.CodeAnalysis;

namespace Genbox.SimpleS3.Core.Abstracts.Enums;

/// <summary>Provides the <c>ValidationStatus</c> API.</summary>
public enum ValidationStatus
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Ok</c> API.</summary>
    Ok,
    /// <summary>Provides the <c>NullInput</c> API.</summary>
    NullInput,
    /// <summary>Provides the <c>WrongLength</c> API.</summary>
    WrongLength,
    /// <summary>Provides the <c>WrongFormat</c> API.</summary>
    WrongFormat,
    /// <summary>Provides the <c>ReservedName</c> API.</summary>
    [SuppressMessage("Naming", "CA1700:Do not name enum values Reserved", Justification = "The public API name describes provider-reserved bucket names.")]
    ReservedName
}