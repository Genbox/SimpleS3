using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class GetObjectAnnotationResponse : BaseResponse, IHasContent, IHasRequestCharged, IHasVersionId, IHasETag, IHasLastModified, IHasSse, IHasReplicationStatus
{
    public ContentStream Content { get; internal set; } = ContentStream.Null;
    public bool RequestCharged { get; internal set; }
    public string? VersionId { get; internal set; }
    public string? ETag { get; internal set; }
    public DateTimeOffset? LastModified { get; internal set; }
    public SseAlgorithm SseAlgorithm { get; internal set; }
    public string? SseKmsKeyId { get; internal set; }
    public ReplicationStatus ReplicationStatus { get; internal set; }
    public ChecksumType ChecksumType { get; internal set; }
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }
    public byte[]? Checksum { get; internal set; }

    public void Dispose()
    {
        Content.Dispose();
    }
}