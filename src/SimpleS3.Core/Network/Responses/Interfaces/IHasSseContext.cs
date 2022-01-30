namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasSseContext
{
    /// <summary>If present, specifies the AWS KMS Encryption Context to use for object encryption. The value of this header is
    /// a base64-encoded UTF-8 string holding JSON with the encryption context key-value pairs.</summary>
    string? SseContext { get; }
}