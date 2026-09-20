using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>PutObjectAnnotationResponse</c> API.</summary>
public sealed class PutObjectAnnotationResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasETag, IHasSse
{
    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string? ObjectKey { get; internal set; }

    /// <summary>Provides the <c>AnnotationName</c> API.</summary>
    public string? AnnotationName { get; internal set; }

    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}