using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

/// <summary>Provides the <c>CompleteMultipartUploadResponse</c> API.</summary>
public class CompleteMultipartUploadResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasExpiration, IHasETag, IHasSse, IHasBucketName, IHasObjectKey
{
    /// <summary>The URI that identifies the newly created object.</summary>
    public string Location { get; internal set; } = null!;

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }

    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>The name of the bucket that contains the newly created object.</summary>
    public string BucketName { get; internal set; } = null!;

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>LifeCycleExpiresOn</c> API.</summary>
    public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }

    /// <summary>Provides the <c>LifeCycleRuleId</c> API.</summary>
    public string? LifeCycleRuleId { get; internal set; }

    /// <summary>The object key of the newly created object.</summary>
    public string ObjectKey { get; internal set; } = null!;

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}