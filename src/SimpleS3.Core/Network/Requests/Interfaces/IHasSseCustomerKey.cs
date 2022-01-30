using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>This represents Server Side Encryption (SSE) with Customer key (SSE-C). You provide a key for Amazon to
/// encrypt your data with. Note that Amazon does not keep this kep. Data is encrypted with the key, and then the key is
/// discarded.</summary>
[PublicAPI]
public interface IHasSseCustomerKey : IContainSensitiveMaterial
{
    /// <summary>The algorithm to use for encryption/decryption</summary>
    SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

    /// <summary>Specifies the customer-provided encryption key to use to decrypt the requested object. This value is used to
    /// perform the decryption and then it is discarded; Amazon does not store the key.</summary>
    byte[]? SseCustomerKey { get; set; }

    /// <summary>Specifies the 128-bit MD5 digest of the customer-provided encryption key according to RFC 1321. If this header
    /// is included in your request, Amazon S3 uses it for a message integrity check to ensure that the encryption key was
    /// transmitted without error.</summary>
    byte[]? SseCustomerKeyMd5 { get; set; }
}