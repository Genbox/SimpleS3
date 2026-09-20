using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

/// <summary>Provides the <c>CreateMultipartUploadResponse</c> API.</summary>
public class CreateMultipartUploadResponse : BaseResponse, IHasSse, IHasSseCustomerKey, IHasRequestCharged, IHasAbort, IHasSseContext, IHasUploadId, IHasBucketName, IHasObjectKey
{
    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>AbortsOn</c> API.</summary>
    public DateTimeOffset? AbortsOn { get; internal set; }

    /// <summary>Provides the <c>AbortRuleId</c> API.</summary>
    public string? AbortRuleId { get; internal set; }

    /// <summary>Name of the bucket to which the multipart upload was initiated.</summary>
    public string BucketName { get; internal set; } = null!;

    /// <summary>Object key for which the multipart upload was initiated.</summary>
    public string ObjectKey { get; internal set; } = null!;

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

    /// <summary>Provides the <c>UploadId</c> API.</summary>
    public string UploadId { get; internal set; } = null!;
}