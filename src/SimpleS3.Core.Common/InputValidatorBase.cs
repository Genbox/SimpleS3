using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Common
{
    public abstract class InputValidatorBase : IInputValidator
    {
        protected abstract bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status);

        public bool TryValidateKeyId(string? keyId, out ValidationStatus status)
        {
            if (keyId == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            return TryValidateKeyIdInternal(keyId, out status);
        }

        protected abstract bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status);

        public bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status)
        {
            if (accessKey == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            return TryValidateAccessKeyInternal(accessKey, out status);
        }

        protected abstract bool TryValidateBucketNameInternal(string bucketName, out ValidationStatus status);

        public bool TryValidateBucketName(string? bucketName, out ValidationStatus status)
        {
            if (bucketName == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            return TryValidateBucketNameInternal(bucketName, out status);
        }

        protected abstract bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status);

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
    }
}
