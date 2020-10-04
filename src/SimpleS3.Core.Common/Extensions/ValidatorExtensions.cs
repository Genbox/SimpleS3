using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Enums;

namespace Genbox.SimpleS3.Core.Common.Extensions
{
    public static class ValidatorExtensions
    {
        public static void ValidateKeyId(this IInputValidator validator, string? keyId)
        {
            if (validator.TryValidateKeyId(keyId, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Key id must be 20 in length", nameof(keyId)),
                ValidationStatus.WrongFormat => new ArgumentException("Key id must be all upper case and consist of A to Z and 0 to 9", nameof(keyId)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(keyId)),
                _ => new ArgumentException("Failed to validate key id")
            };
        }

        public static void ValidateAccessKey(this IInputValidator validator, byte[]? accessKey)
        {
            if (validator.TryValidateAccessKey(accessKey, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Access key must be 40 in length", nameof(accessKey)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(accessKey)),
                _ => new ArgumentException("Failed to validate access key")
            };
        }

        public static void ValidateObjectKey(this IInputValidator validator, string? bucketName, ObjectKeyValidationMode mode)
        {
            if (validator.TryValidateObjectKey(bucketName, mode, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Object keys must be less than 1024 characters in length", nameof(bucketName)),
                ValidationStatus.WrongFormat => new ArgumentException("Invalid character in object key. Only a-z, A-Z, 0-9 and !, -, _, ., *, ', ( and ) are allowed", nameof(bucketName)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(bucketName)),
                _ => new ArgumentException("Failed to validate key id")
            };
        }


        public static void ValidateBucketName(this IInputValidator validator, string? bucketName)
        {
            if (validator.TryValidateBucketName(bucketName, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Bucket names must be less than 64 in length", nameof(bucketName)),
                ValidationStatus.WrongFormat => new ArgumentException("Invalid character in object key. Only a-z, 0-9, . and - are allowed", nameof(bucketName)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(bucketName)),
                _ => new ArgumentException("Failed to validate key id")
            };
        }
    }
}