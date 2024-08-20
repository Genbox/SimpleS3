using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2;

public class BackblazeB2InputValidator : InputValidatorBase
{
    protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
    {
        //B2 master keys are 12. Application keys are 25
        if (keyId.Length != 12 && keyId.Length != 25)
        {
            status = ValidationStatus.WrongLength;
            message = "12 / 25";
            return false;
        }

        foreach (char c in keyId)
        {
            if (CharHelper.InRange(c, 'a', 'f') || CharHelper.InRange(c, '0', '9'))
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message)
    {
        if (accessKey.Length != 31)
        {
            status = ValidationStatus.WrongLength;
            message = "31";
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        //https://www.backblaze.com/b2/docs/buckets.html
        //Spec: A bucket name must be at least 6 characters long, and can be at most 50 characters
        if (bucketName.Length is < 6 or > 50)
        {
            status = ValidationStatus.WrongLength;
            message = "6-50";
            return false;
        }

        //Spec: Bucket names that start with "b2-" are reserved for BackBlaze use.
        if (bucketName.StartsWith("b2-", StringComparison.OrdinalIgnoreCase))
        {
            status = ValidationStatus.ReservedName;
            message = bucketName;
            return false;
        }

        //Spec: Bucket names can consist of upper-case letters, lower-case letters, numbers, and "-". No other characters are allowed.
        foreach (char c in bucketName)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9') || c == '-')
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        //https://www.backblaze.com/b2/docs/files.html

        //Spec: Names can be pretty much any UTF-8 string up to 1024 bytes long
        if (objectKey.Length < 1 || Encoding.UTF8.GetByteCount(objectKey) > 1024)
        {
            status = ValidationStatus.WrongLength;
            message = "1-1024";
            return false;
        }

        foreach (char c in objectKey)
        {
            //Spec: No character codes below 32 are allowed. DEL characters (127) are not allowed
            if (CharHelper.InRange(c, (char)0, (char)31) || c == (char)127)
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }
}