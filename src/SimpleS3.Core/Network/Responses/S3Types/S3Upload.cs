using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Upload : IHasStorageClass, IHasUploadId
    {
        /// <summary>The object key</summary>
        public string ObjectKey { get; internal set; }

        /// <summary>
        /// Identifies who initiated the multipart upload. If the initiator is an AWS account, this element provides the same information as the Owner
        /// element. If the initiator is an IAM User, then this element provides the user ARN and display name.
        /// </summary>
        public S3Identity Initiator { get; internal set; }

        /// <summary>Owner of the upload</summary>
        public S3Identity Owner { get; internal set; }

        /// <summary>The user who initiated the upload</summary>
        public DateTimeOffset Initiated { get; internal set; }

        public StorageClass StorageClass { get; internal set; }

        public string UploadId { get; internal set; }

        public override string ToString()
        {
            return $"{ObjectKey} ({UploadId})";
        }
    }
}