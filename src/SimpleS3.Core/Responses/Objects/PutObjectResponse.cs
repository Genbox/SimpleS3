using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects.Properties;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class PutObjectResponse : BaseResponse, ISseProperties, IStorageClassProperties
    {
        /// <summary>
        /// If the expiration is configured for the object (see PUT Bucket lifecycle), the response includes this header. It includes the expiry-date
        /// and rule-id key-value pairs that provide information about object expiration. The value of the rule-id is URL encoded.
        /// </summary>
        public DateTimeOffset? Expiration { get; internal set; }

        /// <summary>The entity tag is a hash of the object. The ETag reflects changes only to the contents of an object, not its metadata.</summary>
        public string ETag { get; internal set; }

        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }

        /// <summary>Provides storage class information of the object. Amazon S3 returns this header for all objects except for Standard storage class objects.</summary>
        public StorageClass StorageClass { get; internal set; }
    }
}