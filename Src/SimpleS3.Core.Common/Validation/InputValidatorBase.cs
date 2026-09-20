using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Helpers;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Common.Validation;

/// <summary>Provides the <c>InputValidatorBase</c> API.</summary>
[SuppressMessage("Design", "CA1062", Justification = "The public InputValidatorBase entry points handle null input before invoking these protected validation routines.")]
[PublicAPI]
public abstract class InputValidatorBase : IInputValidator
{
    /// <summary>Provides the <c>TryValidateKeyId</c> API.</summary>
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

    /// <summary>Provides the <c>TryValidateAccessKey</c> API.</summary>
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

    /// <summary>Validate a bucket name. See https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html#bucketnamingrules for more info</summary>
    /// <returns>True if the bucket name passed validation</returns>
    public bool TryValidateBucketName(string? bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
    {
        Validator.RequireThat(mode != BucketNameValidationMode.Unknown && Enums.BucketNameValidationMode.IsDefined(mode), "Mode must be defined and must not be Unknown");

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

        return mode switch
        {
            BucketNameValidationMode.DnsLabel => TryValidateBucketDns(bucketName, out status, out message),
            BucketNameValidationMode.Unrestricted => TryValidateBlacklisted(bucketName, out status, out message),
            BucketNameValidationMode.Default or BucketNameValidationMode.DefaultStrict => TryValidateBlacklisted(bucketName, out status, out message) && TryValidateBucketNameInternal(bucketName, mode, out status, out message),
            _ => throw new InvalidOperationException("Unsupported validation mode: " + mode)
        };
    }

    /// <summary>Provides the <c>TryValidateObjectKey</c> API.</summary>
    public bool TryValidateObjectKey(string? objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        Validator.RequireThat(mode != ObjectKeyValidationMode.Unknown && Enums.ObjectKeyValidationMode.IsDefined(mode), "Mode must be defined and must not be Unknown");

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

        return mode switch
        {
            ObjectKeyValidationMode.SafeMode => TryValidateObjectSafeOnly(objectKey, out status, out message),
            ObjectKeyValidationMode.AsciiMode => TryValidateObjectAsciiOnly(objectKey, out status, out message),
            ObjectKeyValidationMode.ExtendedAsciiMode => TryValidateObjectExtAsciiOnly(objectKey, out status, out message),
            ObjectKeyValidationMode.Unrestricted => TryValidateBlacklisted(objectKey, out status, out message),
            ObjectKeyValidationMode.Default or ObjectKeyValidationMode.DefaultStrict => TryValidateBlacklisted(objectKey, out status, out message) && TryValidateObjectKeyInternal(objectKey, mode, out status, out message),
            _ => throw new InvalidOperationException("Unsupported validation mode: " + mode)
        };
    }

    /// <summary>Provides the <c>TryValidateKeyIdInternal</c> API.</summary>
    protected abstract bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message);

    /// <summary>Provides the <c>TryValidateAccessKeyInternal</c> API.</summary>
    protected abstract bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message);

    /// <summary>Provides the <c>TryValidateBucketNameInternal</c> API.</summary>
    protected abstract bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message);

    /// <summary>Provides the <c>TryValidateObjectKeyInternal</c> API.</summary>
    protected abstract bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message);

    /// <summary>Provides the <c>TryValidateBlacklisted</c> API.</summary>
    protected static bool TryValidateBlacklisted(string input, out ValidationStatus status, out string? message)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // UTF-16 surrogate code units are only valid as high/low pairs; reject unpaired surrogates before UTF-8 URL encoding.
            if (char.IsHighSurrogate(c) && i + 1 < input.Length && char.IsLowSurrogate(input[i + 1]))
            {
                i++;
                continue;
            }

            if (char.IsSurrogate(c))
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

    /// <summary>Provides the <c>TryValidateObjectSafeOnly</c> API.</summary>
    protected static bool TryValidateObjectSafeOnly(string input, out ValidationStatus status, out string? message)
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

    /// <summary>Provides the <c>TryValidateObjectAsciiOnly</c> API.</summary>
    protected static bool TryValidateObjectAsciiOnly(string input, out ValidationStatus status, out string? message)
    {
        foreach (char c in input)
        {
            if (CharHelper.InRange(c, (char)0, (char)127))
                continue;

            message = c.ToString();
            status = ValidationStatus.WrongFormat;
            return false;
        }

        message = null;
        status = ValidationStatus.Ok;
        return true;
    }

    /// <summary>Provides the <c>TryValidateObjectExtAsciiOnly</c> API.</summary>
    protected static bool TryValidateObjectExtAsciiOnly(string input, out ValidationStatus status, out string? message)
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

    /// <summary>Provides the <c>TryValidateBucketDns</c> API.</summary>
    protected static bool TryValidateBucketDns(string input, out ValidationStatus status, out string? message)
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

            if (input[newPos - 1] == '-')
            {
                message = "-";
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