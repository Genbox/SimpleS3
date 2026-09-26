using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

/// <summary>Provides the <c>UploadPartResponse</c> API.</summary>
public class UploadPartResponse : BaseResponse, IHasStorageClass, IHasSse, IHasBucketKeyEnabled, IHasSseCustomerKey, IHasETag, IHasRequestCharged
{
    /// <summary>Contains the part number set in the request. This is not returned from the server, this is set by SimpleS3.</summary>
    public int PartNumber { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Whether Amazon S3 used an S3 Bucket Key for SSE-KMS.</summary>
    public bool? BucketKeyEnabled { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string ETag { get; internal set; } = null!; //This must not be null

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

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; internal set; }
}