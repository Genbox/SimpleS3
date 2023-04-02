using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasSseCustomerKey
{
    /// <summary>If server-side encryption with a customer-provided encryption key was requested, the response will include
    /// this header confirming the encryption algorithm used.</summary>
    SseCustomerAlgorithm SseCustomerAlgorithm { get; }

    /// <summary>If server-side encryption using customer-provided encryption keys was requested, the response returns this
    /// header to verify the roundtrip message integrity of the customer-provided encryption key.</summary>
    byte[]? SseCustomerKeyMd5 { get; }
}