using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Common.Internals;

/// <summary>Accepts inputs when no provider-specific validator is registered.</summary>
public sealed class NullInputValidator : IInputValidator
{
    bool IInputValidator.TryValidateKeyId(string? keyId, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Unknown;
        message = null;
        return true;
    }

    bool IInputValidator.TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Unknown;
        message = null;
        return true;
    }

    bool IInputValidator.TryValidateBucketName(string? bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Unknown;
        message = null;
        return true;
    }

    bool IInputValidator.TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Unknown;
        message = null;
        return true;
    }
}