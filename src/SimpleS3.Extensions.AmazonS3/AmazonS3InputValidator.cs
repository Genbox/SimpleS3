using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

public class AmazonS3InputValidator : InputValidatorBase
{
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
            if (c >= 'A' || c <= 'Z')
                continue;

            if (c >= '0' || c <= '9')
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

    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        if (objectKey.Length < 1 || objectKey.Length > 1024)
        {
            status = ValidationStatus.WrongLength;
            message = "1-1024";
            return false;
        }

        foreach (char c in objectKey)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9'))
                continue;

            //See https://docs.aws.amazon.com/AmazonS3/latest/dev/UsingMetadata.html
            if (CharHelper.OneOf(c, '/', '!', '-', '_', '.', '*', '\'', '(', ')'))
                continue;

            //0xD800 to 0xDFFF are reserved code points in UTF-16. Since they will always be URL encoded to %EF%BF%BD (the � char) in UTF-8
            if (CharHelper.InRange(c, '\uD800', '\uDFFF'))
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }

            if (mode == ObjectKeyValidationMode.SafeMode)
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }

            if (CharHelper.OneOf(c, '&', '$', '@', '=', ';', ':', '+', ' ', ',', '?'))
                continue;

            if (CharHelper.InRange(c, (char)0, (char)31) || c == (char)127)
                continue;

            if (mode == ObjectKeyValidationMode.AsciiMode)
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }

            if (CharHelper.OneOf(c, '\\', '{', '}', '^', '%', '`', '[', ']', '"', '<', '>', '~', '#', '|'))
                continue;

            if (CharHelper.InRange(c, (char)128, (char)255))
                continue;

            if (mode == ObjectKeyValidationMode.Unrestricted)
                continue;

            if (mode == ObjectKeyValidationMode.ExtendedAsciiMode)
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

    /// <summary>
    /// Validates a bucket name according to standard DNS rules. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html#bucketnamingrules for more details.
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="status">Contains the error if validation failed</param>
    /// <returns>True if validation succeeded, false otherwise</returns>
    protected override bool TryValidateBucketNameInternal(string bucketName, out ValidationStatus status, out string? message)
    {
        if (bucketName.Length < 3 || bucketName.Length > 63)
        {
            status = ValidationStatus.WrongLength;
            message = "2-63";
            return false;
        }

        int curPos = 0;
        int end = bucketName.Length;

        do
        {
            //find the dot or hit the end
            int newPos = curPos;
            while (newPos < end)
            {
                if (bucketName[newPos] == '.')
                    break;

                ++newPos;
            }

            if (curPos == newPos || newPos - curPos > 63)
            {
                status = ValidationStatus.WrongLength;
                message = "1-63";
                return false;
            }

            char start = bucketName[curPos];

            if (!CharHelper.InRange(start, 'a', 'z') && !CharHelper.InRange(start, '0', '9'))
            {
                status = ValidationStatus.WrongFormat;
                message = start.ToString();
                return false;
            }

            curPos++;

            //check the label content
            while (curPos < newPos)
            {
                char c = bucketName[curPos++];

                if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, '0', '9') || c == '-')
                    continue;

                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }

            ++curPos;
        } while (curPos < end);

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }
}