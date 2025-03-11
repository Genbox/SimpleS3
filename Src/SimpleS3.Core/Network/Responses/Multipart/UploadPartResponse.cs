using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

public class UploadPartResponse : BaseResponse, IHasStorageClass, IHasSse, IHasSseCustomerKey, IHasETag, IHasRequestCharged
{
    /// <summary>Contains the part number set in the request. This is not returned from the server, this is set by SimpleS3.</summary>
    public int PartNumber { get; internal set; }

    public string ETag { get; internal set; } //This must not be null
    public bool RequestCharged { get; internal set; }
    public SseAlgorithm SseAlgorithm { get; internal set; }
    public string? SseKmsKeyId { get; internal set; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
    public byte[]? SseCustomerKeyMd5 { get; internal set; }
    public StorageClass StorageClass { get; internal set; }
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }
    public byte[]? Checksum { get; internal set; }
}