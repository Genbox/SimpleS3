using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Version</c> API.</summary>
public class S3Version(string objectKey, string? versionId, bool isLatest, DateTimeOffset lastModified, string etag, int size, S3Identity? owner, StorageClass storageClass, ChecksumAlgorithm checksumAlgorithm, ChecksumType checksumType) : IHasObjectKey, IHasVersionId
{
    /// <summary>Provides the <c>Etag</c> API.</summary>
    public string Etag { get; } = etag;
    /// <summary>Provides the <c>IsLatest</c> API.</summary>
    public bool IsLatest { get; } = isLatest;
    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; } = lastModified;
    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity? Owner { get; } = owner;
    /// <summary>Provides the <c>Size</c> API.</summary>
    public int Size { get; } = size;
    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; } = storageClass;
    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; internal set; } = objectKey;
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; } = versionId;
    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; } = checksumAlgorithm;
    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; } = checksumType;
    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => ObjectKey;
}