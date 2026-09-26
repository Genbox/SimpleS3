using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>CopyObjectResponse</c> API.</summary>
public class CopyObjectResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasSse, IHasBucketKeyEnabled, IHasSseContext, IHasSseCustomerKey, IHasExpiration, IHasETag, IHasLastModified
{
    /// <summary>The type of checksum returned for the copied object.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>The algorithm used for the copied object's checksum.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>The decoded checksum of the copied object.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Provides the <c>NewVersionId</c> API.</summary>
    public string? NewVersionId { get; internal set; }

    /// <summary>Whether Amazon S3 used an S3 Bucket Key for SSE-KMS.</summary>
    public bool? BucketKeyEnabled { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>LifeCycleExpiresOn</c> API.</summary>
    public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }

    /// <summary>Provides the <c>LifeCycleRuleId</c> API.</summary>
    public string? LifeCycleRuleId { get; internal set; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset? LastModified { get; internal set; }

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

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}