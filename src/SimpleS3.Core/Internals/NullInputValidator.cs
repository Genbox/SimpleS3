using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Internals
{
    public class NullInputValidator : IInputValidator
    {
        public bool TryValidateKeyId(string? keyId, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Unknown;
            message = null;
            return true;
        }

        public bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Unknown;
            message = null;
            return true;
        }

        public bool TryValidateBucketName(string? bucketName, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Unknown;
            message = null;
            return true;
        }

        public bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Unknown;
            message = null;
            return true;
        }
    }
}