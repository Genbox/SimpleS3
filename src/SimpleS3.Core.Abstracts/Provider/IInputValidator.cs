using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts.Provider
{
    public interface IInputValidator
    {
        bool TryValidateKeyId(string? keyId, out ValidationStatus status);
        bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status);
        bool TryValidateBucketName(string? bucketName, out ValidationStatus status);
        bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status);
    }
}