using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart
{
    public class CompleteMultipartUploadResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasExpiration, IHasETag, IHasSse
    {
        /// <summary>The URI that identifies the newly created object.</summary>
        public string Location { get; internal set; }

        /// <summary>The name of the bucket that contains the newly created object.</summary>
        public string BucketName { get; internal set; }

        /// <summary>The object key of the newly created object.</summary>
        public string ObjectKey { get; internal set; }

        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }

        public string ETag { get; internal set; }
        public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }
        public string LifeCycleRuleId { get; internal set; }
        public bool RequestCharged { get; internal set; }
        public SseAlgorithm? SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public string VersionId { get; internal set; }
    }
}