using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class GetObjectAttributesResponse : BaseResponse, IHasDeleteMarker, IHasRequestCharged, IHasStorageClass, IHasVersionId
{
    public GetObjectAttributesResponse() => Parts = new List<S3ObjectAttributePart>();
    public string? ETag { get; internal set; }
    public bool IsDeleteMarker { get; internal set; }
    public bool RequestCharged { get; internal set; }
    public string? VersionId { get; internal set; }
    public StorageClass StorageClass { get; internal set; }
    public DateTimeOffset? LastModified { get; internal set; }
    public long? ObjectSize { get; internal set; }
    public ChecksumType ChecksumType { get; internal set; }
    public byte[]? ChecksumCrc32 { get; internal set; }
    public byte[]? ChecksumCrc32C { get; internal set; }
    public byte[]? ChecksumCrc64Nvme { get; internal set; }
    public byte[]? ChecksumSha1 { get; internal set; }
    public byte[]? ChecksumSha256 { get; internal set; }
    public int TotalPartsCount { get; internal set; }
    public int PartNumberMarker { get; internal set; }
    public int NextPartNumberMarker { get; internal set; }
    public int MaxParts { get; internal set; }
    public bool IsTruncated { get; internal set; }
    public IList<S3ObjectAttributePart> Parts { get; }
}