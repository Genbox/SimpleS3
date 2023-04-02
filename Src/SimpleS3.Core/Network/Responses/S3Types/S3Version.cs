using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Version : IHasObjectKey, IHasVersionId
{
    public S3Version(string objectKey, string? versionId, bool isLatest, DateTimeOffset lastModified, string etag, int size, S3Identity? owner, StorageClass storageClass)
    {
        ObjectKey = objectKey;
        VersionId = versionId;
        IsLatest = isLatest;
        LastModified = lastModified;
        Etag = etag;
        Size = size;
        Owner = owner;
        StorageClass = storageClass;
    }

    public string Etag { get; }
    public bool IsLatest { get; }
    public DateTimeOffset LastModified { get; }
    public S3Identity? Owner { get; }
    public int Size { get; }
    public StorageClass StorageClass { get; }
    public string ObjectKey { get; internal set; }
    public string? VersionId { get; }
}