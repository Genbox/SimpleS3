using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Common.Extensions;

public static class ValidatorExtensions
{
    public static void ValidateKeyIdAndThrow(this IInputValidator validator, string? keyId)
    {
        if (validator.TryValidateKeyId(keyId, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid key id: " + ValidationMessages.GetMessage(status, message), nameof(keyId));
    }

    public static void ValidateAccessKeyAndThrow(this IInputValidator validator, byte[]? accessKey)
    {
        if (validator.TryValidateAccessKey(accessKey, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid access key: " + ValidationMessages.GetMessage(status, message), nameof(accessKey));
    }

    public static void ValidateObjectKey(this IInputValidator validator, string? objectKey, ObjectKeyValidationMode mode)
    {
        if (validator.TryValidateObjectKey(objectKey, mode, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid object key: " + ValidationMessages.GetMessage(status, message), nameof(objectKey));
    }

    public static void ValidateBucketName(this IInputValidator validator, string? bucketName)
    {
        if (validator.TryValidateBucketName(bucketName, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid bucket name: " + ValidationMessages.GetMessage(status, message), nameof(bucketName));
    }
}