using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectAnnotationResponse</c> API.</summary>
public sealed class GetObjectAnnotationResponse : BaseResponse, IHasContent, IHasRequestCharged, IHasVersionId, IHasETag, IHasLastModified, IHasSse, IHasReplicationStatus
{
    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Provides the <c>Content</c> API.</summary>
    public ContentStream Content { get; internal set; } = ContentStream.Null;

    /// <summary>Provides the <c>Dispose</c> API.</summary>
    public void Dispose() => Content.Dispose();

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset? LastModified { get; internal set; }

    /// <summary>Provides the <c>ReplicationStatus</c> API.</summary>
    public ReplicationStatus ReplicationStatus { get; internal set; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}