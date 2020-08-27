using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common
{
    public static class InputValidator
    {
        public static bool TryValidateKeyId(string? keyId, out ValidationStatus status)
        {
            if (string.IsNullOrEmpty(keyId))
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (keyId!.Length != 20)
            {
                status = ValidationStatus.WrongLength;
                return false;
            }

            foreach (char c in keyId)
            {
                if (c >= 'A' || c <= 'Z')
                    continue;

                if (c >= '0' || c <= '9')
                    continue;

                status = ValidationStatus.WrongFormat;
                return false;
            }

            status = ValidationStatus.Ok;
            return true;
        }

        public static void ValidateKeyId(string? keyId)
        {
            if (TryValidateKeyId(keyId, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Key id must be 20 in length", nameof(keyId)),
                ValidationStatus.WrongFormat => new ArgumentException("Key id must be all upper case and consist of A to Z and 0 to 9", nameof(keyId)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(keyId)),
                _ => new ArgumentException("Failed to validate key id")
            };
        }

        public static bool TryValidateAccessKey(byte[]? accessKey, out ValidationStatus status)
        {
            if (accessKey == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (accessKey.Length != 40)
            {
                status = ValidationStatus.WrongLength;
                return false;
            }

            status = ValidationStatus.Ok;
            return true;
        }

        public static void ValidateAccessKey(byte[]? accessKey)
        {
            if (TryValidateAccessKey(accessKey, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Access key must be 40 in length", nameof(accessKey)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(accessKey)),
                _ => new ArgumentException("Failed to validate access key")
            };
        }

        public static bool TryValidateObjectKey(string? objectKey, KeyValidationMode mode, out ValidationStatus status)
        {
            if (string.IsNullOrEmpty(objectKey))
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (objectKey!.Length > 1024)
            {
                status = ValidationStatus.WrongLength;
                return false;
            }

            if (mode == KeyValidationMode.Unrestricted)
            {
                status = ValidationStatus.Ok;
                return true;
            }

            foreach (char c in objectKey)
            {
                if (CharHelper.InRange(c, 'a', 'z'))
                    continue;

                if (CharHelper.InRange(c, 'A', 'Z'))
                    continue;

                if (CharHelper.InRange(c, '0', '9'))
                    continue;

                //See https://docs.aws.amazon.com/AmazonS3/latest/dev/UsingMetadata.html
                if (CharHelper.OneOf(c, '/', '!', '-', '_', '.', '*', '\'', '(', ')'))
                    continue;

                //0xD800 to 0xDFFF are reserved code points in UTF-16. Since they will always be URL encoded to %EF%BF%BD (the � char) in UTF-8
                if (CharHelper.InRange(c, '\uD800', '\uDFFF'))
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }

                if (mode == KeyValidationMode.SafeMode)
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }

                if (CharHelper.OneOf(c, '&', '$', '@', '=', ';', ':', '+', ' ', ',', '?'))
                    continue;

                if (CharHelper.InRange(c, (char)0, (char)31) || c == (char)127)
                    continue;

                if (mode == KeyValidationMode.AsciiMode)
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }

                if (CharHelper.OneOf(c, '\\', '{', '}', '^', '%', '`', '[', ']', '"', '<', '>', '~', '#', '|'))
                    continue;

                if (CharHelper.InRange(c, (char)128, (char)255))
                    continue;

                if (mode == KeyValidationMode.ExtendedAsciiMode)
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }
            }

            status = ValidationStatus.Ok;
            return true;
        }

        public static void ValidateObjectKey(string? bucketName, KeyValidationMode mode)
        {
            if (TryValidateObjectKey(bucketName, mode, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Object keys must be less than 1024 characters in length", nameof(bucketName)),
                ValidationStatus.WrongFormat => new ArgumentException("Invalid character in object key. Only a-z, A-Z, 0-9 and !, -, _, ., *, ', ( and ) are allowed", nameof(bucketName)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(bucketName)),
                _ => new ArgumentException("Failed to validate key id")
            };
        }

        /// <summary>
        /// Validates a bucket name according to standard DNS rules. See
        /// https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html#bucketnamingrules for more details.
        /// </summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="status">Contains the error if validation failed</param>
        /// <returns>True if validation succeeded, false otherwise</returns>
        public static bool TryValidateBucketName(string? bucketName, out ValidationStatus status)
        {
            if (bucketName == null)
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (bucketName.Length < 3 || bucketName.Length > 63)
            {
                status = ValidationStatus.WrongLength;
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
                    return false;
                }

                char start = bucketName[curPos];

                if (!CharHelper.InRange(start, 'a', 'z') && !CharHelper.InRange(start, '0', '9'))
                {
                    status = ValidationStatus.WrongFormat;
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
                    return false;
                }

                ++curPos;
            } while (curPos < end);

            status = ValidationStatus.Ok;
            return true;
        }

        public static void ValidateBucketName(string? bucketName)
        {
            if (TryValidateBucketName(bucketName, out ValidationStatus status))
                return;

            throw status switch
            {
                ValidationStatus.WrongLength => new ArgumentException("Bucket names must be less than 64 in length", nameof(bucketName)),
                ValidationStatus.WrongFormat => new ArgumentException("Invalid character in object key. Only a-z, 0-9, . and - are allowed", nameof(bucketName)),
                ValidationStatus.NullInput => new ArgumentNullException(nameof(bucketName)),
                _ => new ArgumentException("Failed to validate key id")
            };
        }
    }
}