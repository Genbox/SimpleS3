﻿using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Object : IHasStorageClass, IHasETag, IHasObjectKey
{
    public S3Object(string objectKey, DateTimeOffset lastModifiedOn, long size, S3Identity? owner, string? eTag, StorageClass storageClass)
    {
        ObjectKey = objectKey;
        LastModifiedOn = lastModifiedOn;
        Size = size;
        Owner = owner;
        ETag = eTag;
        StorageClass = storageClass;
    }

    public DateTimeOffset LastModifiedOn { get; }
    public long Size { get; }

    public S3Identity? Owner { get; }
    public string? ETag { get; }

    public string ObjectKey { get; internal set; }
    public StorageClass StorageClass { get; }

    public override string ToString() => ObjectKey;
}