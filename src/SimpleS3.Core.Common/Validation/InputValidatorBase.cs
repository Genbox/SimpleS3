using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Common.Validation
{
    public abstract class InputValidatorBase : IInputValidator
    {
        public bool TryValidateKeyId(string? keyId, out ValidationStatus status)
        {
            if (keyId == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            return TryValidateKeyIdInternal(keyId, out status);
        }

        public bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status)
        {
            if (accessKey == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            return TryValidateAccessKeyInternal(accessKey, out status);
        }

        public bool TryValidateBucketName(string? bucketName, out ValidationStatus status)
        {
            if (bucketName == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            return TryValidateBucketNameInternal(bucketName, out status);
        }

        public bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status)
        {
            if (objectKey == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (mode == ObjectKeyValidationMode.Disabled)
            {
                status = ValidationStatus.Ok;
                return true;
            }

            return TryValidateObjectKeyInternal(objectKey, mode, out status);
        }

        protected abstract bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status);

        protected abstract bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status);

        protected abstract bool TryValidateBucketNameInternal(string bucketName, out ValidationStatus status);

        protected abstract bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status);
    }
}