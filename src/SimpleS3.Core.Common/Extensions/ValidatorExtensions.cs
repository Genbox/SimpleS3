using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;

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
                ValidationStatus.WrongLength => new ArgumentException("Key id must be the correct length", nameof(keyId)),
                ValidationStatus.WrongFormat => new ArgumentException("Key id must be in the correct format", nameof(keyId)),
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
                ValidationStatus.WrongLength => new ArgumentException("Access key must be the correct length", nameof(accessKey)),
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
                ValidationStatus.WrongLength => new ArgumentException("Object key must be the correct length", nameof(bucketName)),
                ValidationStatus.WrongFormat => new ArgumentException("Invalid character in object key", nameof(bucketName)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(bucketName)),
                _ => new ArgumentException("Failed to validate object key")
            };
        }

        public static void ValidateBucketName(this IInputValidator validator, string? bucketName)
        {
            if (validator.TryValidateBucketName(bucketName, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Bucket name must be the correct length", nameof(bucketName)),
                ValidationStatus.WrongFormat => new ArgumentException("Invalid character in bucket name", nameof(bucketName)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(bucketName)),
                _ => new ArgumentException("Failed to validate bucket name")
            };
        }
    }
}