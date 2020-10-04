namespace Genbox.SimpleS3.Core.Abstracts.Enums
{
    public enum ObjectKeyValidationMode
    {
        Unknown = 0,
        Unrestricted,
        ExtendedAsciiMode,
        AsciiMode,
        SafeMode,
        ForceSafeEncoding
    }
}