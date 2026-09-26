using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

/// <summary>Provides the <c>UploadPartCopyResponse</c> API.</summary>
public sealed class UploadPartCopyResponse : BaseResponse, IHasETag, IHasRequestCharged, IHasSse, IHasSseCustomerKey
{
    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; internal set; }

    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int PartNumber { get; internal set; }

    /// <summary>The version ID of the source object that was copied.</summary>
    public string? CopySourceVersionId { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string ETag { get; internal set; } = null!;

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; internal set; }

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; internal set; }
}