using System.Text.RegularExpressions;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.Wasabi;

public class WasabiInputValidator : InputValidatorBase
{
    private readonly Regex _ipRegex = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
    {
        if (keyId.Length != 20)
        {
            status = ValidationStatus.WrongLength;
            message = null;
            return false;
        }

        foreach (char c in keyId)
        {
            if (CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9'))
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
        if (accessKey.Length != 40)
        {
            status = ValidationStatus.WrongLength;
            message = "40";
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        //Source: https://wasabi.com/wp-content/themes/wasabi/docs/User_Guide/topics/Creating_a_Bucket.htm

        //Spec: A bucket name can consist of 3 to 63 characters
        if (bucketName.Length is < 3 or > 63)
        {
            status = ValidationStatus.WrongLength;
            message = "3-63";
            return false;
        }

        //Spec: lowercase letters, numbers, periods, and dashes.
        foreach (char c in bucketName)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, '0', '9') || CharHelper.OneOf(c, '.', '-'))
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        //Spec: The name must begin with a lower­case letter or number.
        char start = bucketName[0];
        if (!CharHelper.InRange(start, 'a', 'z') && !CharHelper.InRange(start, '0', '9'))
        {
            status = ValidationStatus.WrongFormat;
            message = start.ToString();
            return false;
        }

        //Spec: The name must begin with a lower­case letter or number.
        char end = bucketName[bucketName.Length - 1];
        if (!CharHelper.InRange(end, 'a', 'z') && !CharHelper.InRange(end, '0', '9'))
        {
            status = ValidationStatus.WrongFormat;
            message = end.ToString();
            return false;
        }

        //Spec: name cannot be formatted as an IP address (123.45.678.90)
        if (_ipRegex.IsMatch(bucketName))
        {
            status = ValidationStatus.WrongFormat;
            message = bucketName;
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        //Source: https://wasabi.com/wp-content/themes/wasabi/docs/User_Guide/topics/Storing_Objects_in_a_Bucket.htm

        if (objectKey.Length < 1 || Constants.Utf8NoBom.GetByteCount(objectKey) > 1024)
        {
            status = ValidationStatus.WrongLength;
            message = "1-1024";
            return false;
        }

        if (mode == ObjectKeyValidationMode.DefaultStrict)
        {
            //Spec: Avoid the use of the following special characters in a file name:
            // % (percent)
            // < (less than symbol)
            // > (greater than symbol)
            // \ (backslash)
            // # (pound sign)
            // ? (question mark)

            foreach (char c in objectKey)
            {
                if (CharHelper.OneOf(c, '%', '<', '>', '\\', '#', '?'))
                {
                    status = ValidationStatus.WrongFormat;
                    message = c.ToString();
                    return false;
                }
            }
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }
}