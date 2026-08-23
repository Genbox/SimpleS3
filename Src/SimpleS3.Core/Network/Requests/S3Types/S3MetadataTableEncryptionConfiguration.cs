using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3MetadataTableEncryptionConfiguration(SseAlgorithm sseAlgorithm)
{
    public SseAlgorithm SseAlgorithm { get; set; } = sseAlgorithm;
    public string? KmsKeyArn { get; set; }
}