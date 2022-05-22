using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Validation;

public abstract class InputValidatorBase : IInputValidator
{
    public bool TryValidateKeyId(string? keyId, out ValidationStatus status, out string? message)
    {
        if (keyId == null)
        {
            status = ValidationStatus.NullInput;
            message = null;
            return false;
        }

        return TryValidateKeyIdInternal(keyId, out status, out message);
    }

    public bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status, out string? message)
    {
        if (accessKey == null)
        {
            status = ValidationStatus.NullInput;
            message = null;
            return false;
        }

        return TryValidateAccessKeyInternal(accessKey, out status, out message);
    }

    /// <summary>Validate a bucket name. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html#bucketnamingrules for more info</summary>
    /// <returns>True if the bucket name passed validation</returns>
    public bool TryValidateBucketName(string? bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        Validator.RequireValidEnum(mode, "Mode must not be Unknown");

        if (mode == BucketNameValidationMode.Disabled)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        if (bucketName == null)
        {
            status = ValidationStatus.NullInput;
            message = null;
            return false;
        }

        switch (mode)
        {
            case BucketNameValidationMode.DnsLabel:
                return TryValidateBucketDns(bucketName, out status, out message);
            case BucketNameValidationMode.Unrestricted:
                return TryValidateBlacklisted(bucketName, out status, out message);
            case BucketNameValidationMode.Default:
            case BucketNameValidationMode.DefaultStrict:
                return TryValidateBlacklisted(bucketName, out status, out message) && TryValidateBucketNameInternal(bucketName, mode, out status, out message);
            default:
                throw new InvalidOperationException("Unsupported validation mode: " + mode);
        }
    }

    public bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        Validator.RequireValidEnum(mode, "Mode must not be Unknown");

        if (mode == ObjectKeyValidationMode.Disabled)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        if (objectKey == null)
        {
            status = ValidationStatus.NullInput;
            message = null;
            return false;
        }

        switch (mode)
        {
            case ObjectKeyValidationMode.SafeMode:
                return TryValidateObjectSafeOnly(objectKey, out status, out message);
            case ObjectKeyValidationMode.AsciiMode:
                return TryValidateObjectAsciiOnly(objectKey, out status, out message);
            case ObjectKeyValidationMode.ExtendedAsciiMode:
                return TryValidateObjectExtAsciiOnly(objectKey, out status, out message);
            case ObjectKeyValidationMode.Unrestricted:
                return TryValidateBlacklisted(objectKey, out status, out message);
            case ObjectKeyValidationMode.Default:
            case ObjectKeyValidationMode.DefaultStrict:
                return TryValidateBlacklisted(objectKey, out status, out message) && TryValidateObjectKeyInternal(objectKey, mode, out status, out message);
            default:
                throw new InvalidOperationException("Unsupported validation mode: " + mode);
        }
    }

    protected abstract bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message);

    protected abstract bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message);

    protected abstract bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message);

    protected abstract bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message);

    protected bool TryValidateBlacklisted(string input, out ValidationStatus status, out string? message)
    {
        foreach (char c in input)
        {
            //0xD800 to 0xDFFF are reserved code points in UTF-16. Since they will always be URL encoded to %EF%BF%BD (the � char) in UTF-8
            if (CharHelper.InRange(c, '\uD800', '\uDFFF'))
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }
        }

        message = null;
        status = ValidationStatus.Ok;
        return true;
    }

    protected bool TryValidateObjectSafeOnly(string input, out ValidationStatus status, out string? message)
    {
        foreach (char c in input)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9'))
                continue;

            message = c.ToString();
            status = ValidationStatus.WrongFormat;
            return false;
        }

        message = null;
        status = ValidationStatus.Ok;
        return true;
    }

    protected bool TryValidateObjectAsciiOnly(string input, out ValidationStatus status, out string? message)
    {
        foreach (char c in input)
        {
            if (CharHelper.InRange(c, (char)0, (char)128))
                continue;

            message = c.ToString();
            status = ValidationStatus.WrongFormat;
            return false;
        }

        message = null;
        status = ValidationStatus.Ok;
        return true;
    }

    protected bool TryValidateObjectExtAsciiOnly(string input, out ValidationStatus status, out string? message)
    {
        foreach (char c in input)
        {
            if (CharHelper.InRange(c, (char)0, (char)255))
                continue;

            message = c.ToString();
            status = ValidationStatus.WrongFormat;
            return false;
        }

        message = null;
        status = ValidationStatus.Ok;
        return true;
    }

    protected bool TryValidateBucketDns(string input, out ValidationStatus status, out string? message)
    {
        int curPos = 0;
        int end = input.Length;

        do
        {
            //find the dot or hit the end
            int newPos = curPos;
            while (newPos < end)
            {
                if (input[newPos] == '.')
                    break;

                ++newPos;
            }

            if (curPos == newPos || newPos - curPos > 63)
            {
                message = "1-63";
                status = ValidationStatus.WrongLength;
                return false;
            }

            char start = input[curPos];

            if (!CharHelper.InRange(start, 'a', 'z') && !CharHelper.InRange(start, '0', '9'))
            {
                message = start.ToString();
                status = ValidationStatus.WrongFormat;
                return false;
            }

            curPos++;

            //check the label content
            while (curPos < newPos)
            {
                char c = input[curPos++];

                if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, '0', '9') || c == '-')
                    continue;

                message = c.ToString();
                status = ValidationStatus.WrongFormat;
                return false;
            }

            ++curPos;
        } while (curPos < end);

        message = null;
        status = ValidationStatus.Ok;
        return true;
    }
}