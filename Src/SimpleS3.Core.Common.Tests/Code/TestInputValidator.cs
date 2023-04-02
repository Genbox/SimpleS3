using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Common.Tests.Code;

internal class TestInputValidator : InputValidatorBase
{
    protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        status = ValidationStatus.Ok;
        message = null;
        return true;
    }
}