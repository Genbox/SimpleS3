using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Common.Extensions
{
    public static class ValidationMessages
    {
        public static IDictionary<ValidationStatus, string> Messages = new Dictionary<ValidationStatus, string>
        {
            {ValidationStatus.Ok, string.Empty},
            {ValidationStatus.WrongFormat, "The input was not in the correct format"},
            {ValidationStatus.WrongLength, "The input was not the correct length"},
            {ValidationStatus.NullInput, "You supplied a null input where it is not allowed"},
            {ValidationStatus.ReservedName, "You supplied a name that is reserved"},
            {ValidationStatus.Unknown, "An unknown error occurred"},
        };
    }

    public static class ValidatorExtensions
    {
        public static void ValidateKeyIdAndThrow(this IInputValidator validator, string? keyId)
        {
            if (validator.TryValidateKeyId(keyId, out ValidationStatus status))
                return;

            throw new ArgumentException("Invalid key id: " + ValidationMessages.Messages[status], nameof(keyId));
        }

        public static void ValidateAccessKeyAndThrow(this IInputValidator validator, byte[]? accessKey)
        {
            if (validator.TryValidateAccessKey(accessKey, out ValidationStatus status))
                return;

            throw new ArgumentException("Invalid access key: " + ValidationMessages.Messages[status], nameof(accessKey));
        }

        public static void ValidateObjectKey(this IInputValidator validator, string? objectKey, ObjectKeyValidationMode mode)
        {
            if (validator.TryValidateObjectKey(objectKey, mode, out ValidationStatus status))
                return;

            throw new ArgumentException("Invalid object key: " + ValidationMessages.Messages[status], nameof(objectKey));
        }

        public static void ValidateBucketName(this IInputValidator validator, string? bucketName)
        {
            if (validator.TryValidateBucketName(bucketName, out ValidationStatus status))
                return;

            throw new ArgumentException("Invalid bucket name: " + ValidationMessages.Messages[status], nameof(bucketName));
        }
    }
}