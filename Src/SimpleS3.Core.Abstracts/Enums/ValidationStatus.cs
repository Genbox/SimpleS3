namespace Genbox.SimpleS3.Core.Abstracts.Enums;

public enum ValidationStatus
{
    Unknown = 0,
    Ok,
    NullInput,
    WrongLength,
    WrongFormat,
#pragma warning disable CA1700 // Public API name describes provider-reserved bucket names.
    ReservedName
#pragma warning restore CA1700
}