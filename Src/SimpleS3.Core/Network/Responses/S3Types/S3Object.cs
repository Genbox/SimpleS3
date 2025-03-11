using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Object(string objectKey, DateTimeOffset lastModifiedOn, long size, S3Identity? owner, string? eTag, StorageClass storageClass, ChecksumAlgorithm checksumAlgorithm, ChecksumType checksumType) : IHasStorageClass, IHasETag, IHasObjectKey
{
    public DateTimeOffset LastModifiedOn { get; } = lastModifiedOn;
    public long Size { get; } = size;
    public S3Identity? Owner { get; } = owner;
    public string? ETag { get; } = eTag;
    public string ObjectKey { get; internal set; } = objectKey;
    public StorageClass StorageClass { get; } = storageClass;
    public ChecksumAlgorithm ChecksumAlgorithm { get; } = checksumAlgorithm;
    public ChecksumType ChecksumType { get; } = checksumType;
    public override string ToString() => ObjectKey;
}