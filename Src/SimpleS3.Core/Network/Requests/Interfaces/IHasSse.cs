using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

[PublicAPI]
public interface IHasSse
{
    /// <summary>Specifies the server-side encryption algorithm to use when Amazon S3 creates an object.</summary>
    SseAlgorithm SseAlgorithm { get; set; }

    /// <summary>
    /// If <see cref="SseAlgorithm" /> has a value of AwsKms, this property specifies the id of the AWS Key Management Service (AWS KMS) master encryption key that was used for
    /// the object. Note that if you do not provide a <see cref="SseKmsKeyId" /> Amazon S3 uses the default AWS KMS key to protect the data.
    /// </summary>
    string? SseKmsKeyId { get; set; }

    /// <summary>
    /// If <see cref="Enums.SseAlgorithm" /> has a value of AwsKms, this property specifies the encryption context for the object. The key/value pairs you add will be bound to
    /// the encrypted content and protected, but the pairs themselves will not be encrypted. See https://docs.aws.amazon.com/kms/latest/developerguide/concepts.html#encrypt_context for
    /// more info on the subject.
    /// </summary>
    KmsContextBuilder SseContext { get; }
}