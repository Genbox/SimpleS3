namespace Genbox.SimpleS3.Core.Abstracts.Enums
{
    public enum ValidationStatus
    {
        Unknown = 0,
        Ok,
        NullInput,
        WrongLength,
        WrongFormat,
        ReservedName
    }
}