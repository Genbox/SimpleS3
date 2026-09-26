using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Object</c> API.</summary>
public class S3Object : IHasStorageClass, IHasETag, IHasObjectKey
{
    internal S3Object(string objectKey, DateTimeOffset lastModifiedOn, long size, S3Identity? owner, string? eTag, StorageClass storageClass, IEnumerable<ChecksumAlgorithm> checksumAlgorithms, ChecksumType checksumType)
    {
        LastModifiedOn = lastModifiedOn;
        Size = size;
        Owner = owner;
        ChecksumAlgorithms = new List<ChecksumAlgorithm>(checksumAlgorithms);
        ChecksumType = checksumType;
        ETag = eTag;
        ObjectKey = objectKey;
        StorageClass = storageClass;
    }

    /// <summary>The optional restoration status returned by the service.</summary>
    public S3RestoreStatus? RestoreStatus { get; internal set; }

    /// <summary>Provides the <c>LastModifiedOn</c> API.</summary>
    public DateTimeOffset LastModifiedOn { get; }

    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; }

    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity? Owner { get; }

    /// <summary>All checksum algorithms reported by the service.</summary>
    public IList<ChecksumAlgorithm> ChecksumAlgorithms { get; }

    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; internal set; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => ObjectKey;
}