using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.GenericS3;

/// <summary>Provides the <c>GenericS3InputValidator</c> API.</summary>
public class GenericS3InputValidator : InputValidatorBase
{
    /// <summary>Provides the <c>TryValidateKeyIdInternal</c> API.</summary>
    protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    /// <summary>Provides the <c>TryValidateAccessKeyInternal</c> API.</summary>
    protected override bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    /// <summary>Provides the <c>TryValidateBucketNameInternal</c> API.</summary>
    protected override bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    /// <summary>Provides the <c>TryValidateObjectKeyInternal</c> API.</summary>
    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }
}