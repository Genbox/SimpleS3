using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3ServerSideEncryptionRule
{
    public S3ServerSideEncryptionRule() {}

    public S3ServerSideEncryptionRule(SseAlgorithm sseAlgorithm)
    {
        SseAlgorithm = sseAlgorithm;
    }

    public SseAlgorithm SseAlgorithm { get; set; }
    public string? KmsMasterKeyId { get; set; }
    public bool? BucketKeyEnabled { get; set; }
    public IList<BlockedEncryptionType> BlockedEncryptionTypes { get; } = new List<BlockedEncryptionType>();
}