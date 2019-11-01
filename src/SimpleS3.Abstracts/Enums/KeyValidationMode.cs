namespace Genbox.SimpleS3.Abstracts.Enums
{
    public enum KeyValidationMode
    {
        Unknown = 0,
        Unrestricted,
        ExtendedAsciiMode,
        AsciiMode,
        SafeMode,
        ForceSafeEncoding
    }
}