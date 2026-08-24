using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Object</c> API.</summary>
public class S3Object(string objectKey, DateTimeOffset lastModifiedOn, long size, S3Identity? owner, string? eTag, StorageClass storageClass, ChecksumAlgorithm checksumAlgorithm, ChecksumType checksumType) : IHasStorageClass, IHasETag, IHasObjectKey
{
    /// <summary>Provides the <c>LastModifiedOn</c> API.</summary>
    public DateTimeOffset LastModifiedOn { get; } = lastModifiedOn;
    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; } = size;
    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity? Owner { get; } = owner;
    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; } = eTag;
    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; internal set; } = objectKey;
    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; } = storageClass;
    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; } = checksumAlgorithm;
    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; } = checksumType;
    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => ObjectKey;
}