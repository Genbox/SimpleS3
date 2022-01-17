using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Common.Validation
{
    public abstract class InputValidatorBase : IInputValidator
    {
        public bool TryValidateKeyId(string? keyId, out ValidationStatus status, out string? message)
        {
            if (keyId == null)
            {
                status = ValidationStatus.NullInput;
                message = null;
                return false;
            }

            return TryValidateKeyIdInternal(keyId, out status, out message);
        }

        public bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status, out string? message)
        {
            if (accessKey == null)
            {
                status = ValidationStatus.NullInput;
                message = null;
                return false;
            }

            return TryValidateAccessKeyInternal(accessKey, out status, out message);
        }

        public bool TryValidateBucketName(string? bucketName, out ValidationStatus status, out string? message)
        {
            if (bucketName == null)
            {
                status = ValidationStatus.NullInput;
                message = null;
                return false;
            }

            return TryValidateBucketNameInternal(bucketName, out status, out message);
        }

        public bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
        {
            if (objectKey == null)
            {
                status = ValidationStatus.NullInput;
                message = null;
                return false;
            }

            if (mode == ObjectKeyValidationMode.Disabled)
            {
                status = ValidationStatus.Ok;
                message = null;
                return true;
            }

            return TryValidateObjectKeyInternal(objectKey, mode, out status, out message);
        }

        protected abstract bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message);

        protected abstract bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message);

        protected abstract bool TryValidateBucketNameInternal(string bucketName, out ValidationStatus status, out string? message);

        protected abstract bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message);
    }
}