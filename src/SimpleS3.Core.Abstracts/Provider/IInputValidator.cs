using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts.Provider;

public interface IInputValidator
{
    bool TryValidateKeyId(string? keyId, out ValidationStatus status, out string? message);
    bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status, out string? message);
    bool TryValidateBucketName(string? bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message);
    bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message);
}