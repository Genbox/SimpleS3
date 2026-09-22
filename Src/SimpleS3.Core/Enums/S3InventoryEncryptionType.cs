namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Server-side encryption used for inventory reports.</summary>
public enum S3InventoryEncryptionType
{
    /// <summary>Amazon S3 managed encryption.</summary>
    SseS3,
    /// <summary>AWS Key Management Service encryption.</summary>
    SseKms
}