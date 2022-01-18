using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums;

public enum SseAlgorithm
{
    Unknown = 0,

    /// <summary>
    /// Similar to SSE-S3, but with some additional benefits along with some additional charges for using this service. There are separate
    /// permissions for the use of an envelope key (that is, a key that protects your data's encryption key) that provides added protection against
    /// unauthorized access of your objects in Amazon S3. SSE-KMS also provides you with an audit trail of when your key was used and by whom. Additionally,
    /// you have the option to create and manage encryption keys yourself, or use a default key that is unique to you, the service you're using, and the
    /// Region you're working in.
    /// </summary>
    [EnumValue("aws:kms")]
    AwsKms,

    /// <summary>
    /// Each object is encrypted with a unique key. As an additional safeguard, it encrypts the key itself with a master key that it regularly
    /// rotates.
    /// </summary>
    [EnumValue("AES256")]
    Aes256
}