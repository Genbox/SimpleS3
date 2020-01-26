namespace Genbox.SimpleS3.Core.Abstracts.Enums
{
    public enum SignatureType : byte
    {
        Unknown = 0,
        Unsigned,
        FullSignature,
        StreamingSignature
    }
}