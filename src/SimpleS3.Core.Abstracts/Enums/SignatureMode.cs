namespace Genbox.SimpleS3.Core.Abstracts.Enums
{
    public enum SignatureMode : byte
    {
        Unknown = 0,
        Unsigned,
        FullSignature,
        StreamingSignature
    }
}