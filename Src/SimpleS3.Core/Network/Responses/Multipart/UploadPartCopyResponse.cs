using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

public sealed class UploadPartCopyResponse : BaseResponse, IHasETag, IHasRequestCharged, IHasSse, IHasSseCustomerKey
{
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }
    public byte[]? Checksum { get; internal set; }
    public string ETag { get; internal set; } = null!;
    public DateTimeOffset LastModified { get; internal set; }
    public int PartNumber { get; internal set; }
    public bool RequestCharged { get; internal set; }
    public SseAlgorithm SseAlgorithm { get; internal set; }
    public string? SseKmsKeyId { get; internal set; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
    public byte[]? SseCustomerKeyMd5 { get; internal set; }
}