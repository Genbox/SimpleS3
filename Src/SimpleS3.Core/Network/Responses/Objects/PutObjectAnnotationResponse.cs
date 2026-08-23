using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class PutObjectAnnotationResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasETag, IHasSse
{
    public string? ObjectKey { get; internal set; }
    public string? AnnotationName { get; internal set; }
    public bool RequestCharged { get; internal set; }
    public string? VersionId { get; internal set; }
    public string? ETag { get; internal set; }
    public SseAlgorithm SseAlgorithm { get; internal set; }
    public string? SseKmsKeyId { get; internal set; }
    public ChecksumType ChecksumType { get; internal set; }
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }
    public byte[]? Checksum { get; internal set; }
}