using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Enums;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IInputValidator
    {
        bool TryValidateKeyId(string? keyId, out ValidationStatus status);
        bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status);
        bool TryValidateBucketName(string? bucketName, out ValidationStatus status);
        bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status);
    }
}