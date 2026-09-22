using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Upload</c> API.</summary>
public class S3Upload : IHasStorageClass, IHasUploadId, IHasObjectKey
{
    internal S3Upload(string objectKey, string uploadId, S3Identity? initiator, S3Identity? owner, StorageClass storageClass, DateTimeOffset initiated, ChecksumAlgorithm checksumAlgorithm = ChecksumAlgorithm.Unknown, ChecksumType checksumType = ChecksumType.Unknown)
    {
        Initiator = initiator;
        Owner = owner;
        Initiated = initiated;
        ObjectKey = objectKey;
        StorageClass = storageClass;
        UploadId = uploadId;
        ChecksumAlgorithm = checksumAlgorithm;
        ChecksumType = checksumType;
    }

    /// <summary>
    /// Identifies who initiated the multipart upload. If the initiator is an AWS account, this element provides the same information as the Owner element. If the initiator is an
    /// IAM User, then this element provides the user ARN and display name.
    /// </summary>
    public S3Identity? Initiator { get; }

    /// <summary>Owner of the upload</summary>
    public S3Identity? Owner { get; }

    /// <summary>The user who initiated the upload</summary>
    public DateTimeOffset Initiated { get; }

    /// <summary>The checksum algorithm selected when this multipart upload was initiated.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; }

    /// <summary>How part checksums will be combined for the completed object.</summary>
    public ChecksumType ChecksumType { get; }

    /// <summary>The object key</summary>
    public string ObjectKey { get; internal set; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; }

    /// <summary>Provides the <c>UploadId</c> API.</summary>
    public string UploadId { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => $"{ObjectKey} ({UploadId})";
}