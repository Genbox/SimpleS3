using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Version(string objectKey, string? versionId, bool isLatest, DateTimeOffset lastModified, string etag, int size, S3Identity? owner, StorageClass storageClass) : IHasObjectKey, IHasVersionId
{
    public string Etag { get; } = etag;
    public bool IsLatest { get; } = isLatest;
    public DateTimeOffset LastModified { get; } = lastModified;
    public S3Identity? Owner { get; } = owner;
    public int Size { get; } = size;
    public StorageClass StorageClass { get; } = storageClass;
    public string ObjectKey { get; internal set; } = objectKey;
    public string? VersionId { get; } = versionId;
}