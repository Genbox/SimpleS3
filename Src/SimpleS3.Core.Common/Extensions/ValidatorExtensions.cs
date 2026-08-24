using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Common.Extensions;

/// <summary>Provides the <c>ValidatorExtensions</c> API.</summary>
public static class ValidatorExtensions
{
    /// <summary>Provides the <c>ValidateKeyIdAndThrow</c> API.</summary>
    public static void ValidateKeyIdAndThrow(this IInputValidator validator, string? keyId)
    {
        if (validator.TryValidateKeyId(keyId, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid key id: " + ValidationMessages.GetMessage(status, message), nameof(keyId));
    }

    /// <summary>Provides the <c>ValidateAccessKeyAndThrow</c> API.</summary>
    public static void ValidateAccessKeyAndThrow(this IInputValidator validator, byte[]? accessKey)
    {
        if (validator.TryValidateAccessKey(accessKey, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid access key: " + ValidationMessages.GetMessage(status, message), nameof(accessKey));
    }

    /// <summary>Provides the <c>ValidateObjectKey</c> API.</summary>
    public static void ValidateObjectKey(this IInputValidator validator, string? objectKey, ObjectKeyValidationMode mode)
    {
        if (validator.TryValidateObjectKey(objectKey, mode, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid object key: " + ValidationMessages.GetMessage(status, message), nameof(objectKey));
    }

    /// <summary>Provides the <c>ValidateBucketName</c> API.</summary>
    public static void ValidateBucketName(this IInputValidator validator, BucketNameValidationMode mode, string? bucketName)
    {
        if (validator.TryValidateBucketName(bucketName, mode, out ValidationStatus status, out string? message))
            return;

        throw new ArgumentException("Invalid bucket name: " + ValidationMessages.GetMessage(status, message), nameof(bucketName));
    }
}