using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    [PublicAPI]
    public interface IHasSse
    {
        /// <summary>
        /// If you specified server-side encryption either with an AWS KMS-managed or Amazon S3-managed encryption key in your PUT request, the response
        /// includes this header. It confirms the encryption algorithm that Amazon S3 used to encrypt the object.
        /// </summary>
        SseAlgorithm? SseAlgorithm { get; }

        /// <summary>If present, specifies the ID of the AWS Key Management Service (KMS) customer master key (CMK) that was used for the object.</summary>
        string SseKmsKeyId { get; }
    }
}