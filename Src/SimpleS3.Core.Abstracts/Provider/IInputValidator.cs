using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts.Provider;

/// <summary>Provides the <c>IInputValidator</c> API.</summary>
public interface IInputValidator
{
    /// <summary>Provides the <c>TryValidateKeyId</c> API.</summary>
    bool TryValidateKeyId(string? keyId, out ValidationStatus status, out string? message);
    /// <summary>Provides the <c>TryValidateAccessKey</c> API.</summary>
    bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status, out string? message);
    /// <summary>Provides the <c>TryValidateBucketName</c> API.</summary>
    bool TryValidateBucketName(string? bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message);
    /// <summary>Provides the <c>TryValidateObjectKey</c> API.</summary>
    bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message);
}