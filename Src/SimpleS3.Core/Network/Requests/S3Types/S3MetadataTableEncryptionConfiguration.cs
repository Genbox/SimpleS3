using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3MetadataTableEncryptionConfiguration</c> API.</summary>
public sealed class S3MetadataTableEncryptionConfiguration(SseAlgorithm sseAlgorithm)
{
    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; set; } = sseAlgorithm;

    /// <summary>Provides the <c>KmsKeyArn</c> API.</summary>
    public string? KmsKeyArn { get; set; }
}