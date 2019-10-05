using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface ISseProperties
    {
        /// <summary>
        /// If you specified server-side encryption either with an AWS KMS-managed or Amazon S3-managed encryption key in your PUT request, the response
        /// includes this header. It confirms the encryption algorithm that Amazon S3 used to encrypt the object.
        /// </summary>
        SseAlgorithm SseAlgorithm { get; }

        /// <summary>
        /// If the x-amz-server-side-encryption is present and has the value of aws:kms, this header specifies the ID of the AWS KMS master encryption
        /// key that was used for the object.
        /// </summary>
        string SseKmsKeyId { get; }

        /// <summary>
        /// If server-side encryption with customer-provided encryption keys encryption was requested, the response includes this header that confirms
        /// the encryption algorithm that was used.
        /// </summary>
        SseCustomerAlgorithm SseCustomerAlgorithm { get; }

        /// <summary>
        /// If server-side encryption using customer-provided encryption keys was requested, the response returns this header to verify the roundtrip
        /// message integrity of the customer-provided encryption key.
        /// </summary>
        byte[] SseCustomerKeyMd5 { get; }
    }
}