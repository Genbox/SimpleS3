using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>PutObjectResponse</c> API.</summary>
public class PutObjectResponse : BaseResponse, IHasExpiresOn, IHasETag, IHasSse, IHasBucketKeyEnabled, IHasSseCustomerKey, IHasStorageClass, IHasRequestCharged, IHasVersionId, IHasSseContext, IHasExpiration
{
    /// <summary>The resulting object size in bytes returned by a directory-bucket append, or null when absent.</summary>
    public long? Size { get; internal set; }

    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Whether Amazon S3 used an S3 Bucket Key for SSE-KMS.</summary>
    public bool? BucketKeyEnabled { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>LifeCycleExpiresOn</c> API.</summary>
    public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }

    /// <summary>Provides the <c>LifeCycleRuleId</c> API.</summary>
    public string? LifeCycleRuleId { get; internal set; }

    /// <summary>Provides the <c>ExpiresOn</c> API.</summary>
    public DateTimeOffset? ExpiresOn { get; internal set; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; internal set; }

    /// <summary>Provides the <c>SseContext</c> API.</summary>
    public string? SseContext { get; internal set; }

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; internal set; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}