﻿using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Upload(string objectKey, string uploadId, S3Identity? initiator, S3Identity? owner, StorageClass storageClass, DateTimeOffset initiated) : IHasStorageClass, IHasUploadId, IHasObjectKey
{
    /// <summary>
    /// Identifies who initiated the multipart upload. If the initiator is an AWS account, this element provides the same information as the Owner element. If the initiator is an
    /// IAM User, then this element provides the user ARN and display name.
    /// </summary>
    public S3Identity? Initiator { get; } = initiator;

    /// <summary>Owner of the upload</summary>
    public S3Identity? Owner { get; } = owner;

    /// <summary>The user who initiated the upload</summary>
    public DateTimeOffset Initiated { get; } = initiated;

    /// <summary>The object key</summary>
    public string ObjectKey { get; internal set; } = objectKey;

    public StorageClass StorageClass { get; } = storageClass;

    public string UploadId { get; } = uploadId;

    public override string ToString() => $"{ObjectKey} ({UploadId})";
}