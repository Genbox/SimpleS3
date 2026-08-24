using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3ServerSideEncryptionRule</c> API.</summary>
public class S3ServerSideEncryptionRule
{
    /// <summary>Provides the <c>S3ServerSideEncryptionRule</c> API.</summary>
    public S3ServerSideEncryptionRule() {}

    /// <summary>Provides the <c>S3ServerSideEncryptionRule</c> API.</summary>
    public S3ServerSideEncryptionRule(SseAlgorithm sseAlgorithm)
    {
        SseAlgorithm = sseAlgorithm;
    }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; set; }
    /// <summary>Provides the <c>KmsMasterKeyId</c> API.</summary>
    public string? KmsMasterKeyId { get; set; }
    /// <summary>Provides the <c>BucketKeyEnabled</c> API.</summary>
    public bool? BucketKeyEnabled { get; set; }
    /// <summary>Provides the <c>BlockedEncryptionTypes</c> API.</summary>
    public IList<BlockedEncryptionType> BlockedEncryptionTypes { get; } = new List<BlockedEncryptionType>();
}