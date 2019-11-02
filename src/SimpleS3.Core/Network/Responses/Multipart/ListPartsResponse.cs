using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart
{
    public class ListPartsResponse : BaseResponse, IHasRequestCharged, IHasAbort, IHasTruncated, IHasStorageClass, IHasUploadId
    {
        /// <summary>Name of the bucket to which the multipart upload was initiated.</summary>
        public string BucketName { get; internal set; }

        /// <summary>Object key for which the multipart upload was initiated.</summary>
        public string ObjectKey { get; internal set; }

        /// <summary>
        /// When a list is truncated, this element specifies the last part in the list, as well as the value to use for the part-number-marker request
        /// parameter in a subsequent request.
        /// </summary>
        public int PartNumberMarker { get; internal set; }

        /// <summary>
        /// When a list is truncated, this element specifies the last part in the list, as well as the value to use for the part-number-marker request
        /// parameter in a subsequent request.
        /// </summary>
        public int NextPartNumberMarker { get; internal set; }

        /// <summary>Maximum number of parts that were allowed in the response.</summary>
        public int MaxParts { get; internal set; }

        /// <summary>
        /// Identifies the object owner, after the object is created. If multipart upload is initiated by an IAM user, this element provides the parent
        /// account ID and display name.
        /// </summary>
        public S3Identity Owner { get; internal set; }

        /// <summary>
        /// Identifies who initiated the multipart upload. If the initiator is an AWS account, this element provides the same information as the Owner
        /// element. If the initiator is an IAM User, then this element provides the user ARN and display name.
        /// </summary>
        public S3Identity Initiator { get; internal set; }

        /// <summary>The list of parts</summary>
        public IList<S3Part> Parts { get; internal set; }

        public DateTimeOffset? AbortsOn { get; internal set; }
        public string AbortRuleId { get; internal set; }
        public bool RequestCharged { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
        public bool IsTruncated { get; internal set; }
        public EncodingType EncodingType { get; internal set; }
        public string UploadId { get; internal set; }
    }
}