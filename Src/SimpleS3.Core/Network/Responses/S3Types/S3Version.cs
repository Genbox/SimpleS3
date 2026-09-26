using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Version</c> API.</summary>
public class S3Version : IHasObjectKey, IHasVersionId
{
    internal S3Version(string objectKey, string? versionId, bool isLatest, DateTimeOffset lastModified, string etag, long size, S3Identity? owner, StorageClass storageClass, IEnumerable<ChecksumAlgorithm> checksumAlgorithms, ChecksumType checksumType)
    {
        Etag = etag;
        IsLatest = isLatest;
        LastModified = lastModified;
        Owner = owner;
        Size = size;
        StorageClass = storageClass;
        ChecksumAlgorithms = new List<ChecksumAlgorithm>(checksumAlgorithms);
        ChecksumType = checksumType;
        ObjectKey = objectKey;
        VersionId = versionId;
    }

    /// <summary>Creates a version using the legacy 32-bit size type.</summary>
    internal S3Version(string objectKey, string? versionId, bool isLatest, DateTimeOffset lastModified, string etag, int size, S3Identity? owner, StorageClass storageClass, IEnumerable<ChecksumAlgorithm> checksumAlgorithms, ChecksumType checksumType)
        : this(objectKey, versionId, isLatest, lastModified, etag, (long)size, owner, storageClass, checksumAlgorithms, checksumType) {}

    /// <summary>The optional restoration status returned by the service.</summary>
    public S3RestoreStatus? RestoreStatus { get; internal set; }

    /// <summary>Provides the <c>Etag</c> API.</summary>
    public string Etag { get; }

    /// <summary>Provides the <c>IsLatest</c> API.</summary>
    public bool IsLatest { get; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; }

    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity? Owner { get; }

    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; }

    /// <summary>All checksum algorithms reported by the service.</summary>
    public IList<ChecksumAlgorithm> ChecksumAlgorithms { get; }

    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => ObjectKey;
}