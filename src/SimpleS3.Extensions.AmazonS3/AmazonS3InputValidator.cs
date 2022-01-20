using System.Text.RegularExpressions;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

public class AmazonS3InputValidator : InputValidatorBase
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
        //Source: https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html

        //Spec: Bucket names must be between 3 and 63 characters long.
        if (bucketName.Length < 3 || bucketName.Length > 63)
        {
            status = ValidationStatus.WrongLength;
            message = "3-63";
            return false;
        }

        //Spec: Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).
        foreach (char c in bucketName)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, '0', '9') || CharHelper.OneOf(c, '.', '-'))
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        //Spec: Bucket names must begin and end with a letter or number.
        char start = bucketName[0];
        if (!CharHelper.InRange(start, 'a', 'z') && !CharHelper.InRange(start, '0', '9'))
        {
            status = ValidationStatus.WrongFormat;
            message = start.ToString();
            return false;
        }

        //Spec: Bucket names must begin and end with a letter or number.
        char end = bucketName[bucketName.Length - 1];
        if (!CharHelper.InRange(end, 'a', 'z') && !CharHelper.InRange(end, '0', '9'))
        {
            status = ValidationStatus.WrongFormat;
            message = end.ToString();
            return false;
        }

        //Spec: Bucket names must not be formatted as an IP address (for example, 192.168.5.4).
        if (_ipRegex.IsMatch(bucketName))
        {
            status = ValidationStatus.WrongFormat;
            message = bucketName;
            return false;
        }

        //Spec: Bucket names must not start with the prefix xn--.
        if (bucketName.StartsWith("xn--", StringComparison.Ordinal))
        {
            status = ValidationStatus.WrongFormat;
            message = "xn--";
            return false;
        }

        //Spec: Bucket names must not end with the suffix -s3alias. This suffix is reserved for access point alias names. For more information, see Using a bucket-style alias for your access point.
        if (bucketName.EndsWith("-s3alias", StringComparison.Ordinal))
        {
            status = ValidationStatus.WrongFormat;
            message = "-s3alias";
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        //Source: https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html

        //Spec: The name for a key is a sequence of Unicode characters whose UTF-8 encoding is at most 1,024 bytes long.
        if (objectKey.Length < 1 || Constants.Utf8NoBom.GetByteCount(objectKey) > 1024)
        {
            status = ValidationStatus.WrongLength;
            message = "1-1024";
            return false;
        }

        //Spec: You can use any UTF-8 character in an object key name. However, using certain characters in key names can cause problems with some applications and protocols.
        foreach (char c in objectKey)
        {
            //Spec: Safe characters
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9'))
                continue;

            //Spec: Safe characters
            if (CharHelper.OneOf(c, '/', '!', '-', '_', '.', '*', '\'', '(', ')'))
                continue;

            if (mode == ObjectKeyValidationMode.DefaultStrict)
            {
                //Spec: Characters that might require special handling
                if (CharHelper.OneOf(c, '&', '$', '@', '=', ';', ':', '+', ' ', ',', '?') || CharHelper.InRange(c, (char)0, (char)31) || c == (char)127)
                {
                    status = ValidationStatus.WrongFormat;
                    message = c.ToString();
                    return false;
                }

                //Spec: Characters to avoid
                if (CharHelper.OneOf(c, '\\', '{', '}', '^', '%', '`', '[', ']', '"', '<', '>', '~', '#', '|'))
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