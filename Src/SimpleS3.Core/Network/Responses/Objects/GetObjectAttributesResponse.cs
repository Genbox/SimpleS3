using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectAttributesResponse</c> API.</summary>
public sealed class GetObjectAttributesResponse : BaseResponse, IHasDeleteMarker, IHasRequestCharged, IHasStorageClass, IHasVersionId
{
    /// <summary>Provides the <c>GetObjectAttributesResponse</c> API.</summary>
    public GetObjectAttributesResponse() => Parts = new List<S3ObjectAttributePart>();
    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }
    /// <summary>Provides the <c>IsDeleteMarker</c> API.</summary>
    public bool IsDeleteMarker { get; internal set; }
    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; internal set; }
    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset? LastModified { get; internal set; }
    /// <summary>Provides the <c>ObjectSize</c> API.</summary>
    public long? ObjectSize { get; internal set; }
    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }
    /// <summary>Provides the <c>ChecksumCrc32</c> API.</summary>
    public byte[]? ChecksumCrc32 { get; internal set; }
    /// <summary>Provides the <c>ChecksumCrc32C</c> API.</summary>
    public byte[]? ChecksumCrc32C { get; internal set; }
    /// <summary>Provides the <c>ChecksumCrc64Nvme</c> API.</summary>
    public byte[]? ChecksumCrc64Nvme { get; internal set; }
    /// <summary>Provides the <c>ChecksumSha1</c> API.</summary>
    public byte[]? ChecksumSha1 { get; internal set; }
    /// <summary>Provides the <c>ChecksumSha256</c> API.</summary>
    public byte[]? ChecksumSha256 { get; internal set; }
    /// <summary>Provides the <c>TotalPartsCount</c> API.</summary>
    public int TotalPartsCount { get; internal set; }
    /// <summary>Provides the <c>PartNumberMarker</c> API.</summary>
    public int PartNumberMarker { get; internal set; }
    /// <summary>Provides the <c>NextPartNumberMarker</c> API.</summary>
    public int NextPartNumberMarker { get; internal set; }
    /// <summary>Provides the <c>MaxParts</c> API.</summary>
    public int MaxParts { get; internal set; }
    /// <summary>Provides the <c>IsTruncated</c> API.</summary>
    public bool IsTruncated { get; internal set; }
    /// <summary>Provides the <c>Parts</c> API.</summary>
    public IList<S3ObjectAttributePart> Parts { get; }
}