using System;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Utils.Enums;
using Genbox.SimpleS3.Utils.Helpers;

namespace Genbox.SimpleS3.Utils
{
    public static class InputValidator
    {
        public static bool TryValidateKeyId(string keyId, out ValidationStatus status)
        {
            if (string.IsNullOrEmpty(keyId))
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (keyId.Length != 20)
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

        public static void ValidateKeyId(string keyId)
        {
            Validator.RequireNotNull(keyId, nameof(keyId));

            if (TryValidateKeyId(keyId, out ValidationStatus status))
                return;

            switch (status)
            {
                case ValidationStatus.WrongLength:
                    throw new ArgumentException("Key id must be 20 in length", nameof(keyId));
                case ValidationStatus.WrongFormat:
                    throw new ArgumentException("Key id must be all upper case and consist of A to Z and 0 to 9", nameof(keyId));
                default:
                    throw new ArgumentException("Failed to validate key id");
            }
        }

        public static bool TryValidateAccessKey(byte[] accessKey, out ValidationStatus status)
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

        public static void ValidateAccessKey(byte[] accessKey)
        {
            Validator.RequireNotNull(accessKey, nameof(accessKey));

            if (TryValidateAccessKey(accessKey, out ValidationStatus status))
                return;

            switch (status)
            {
                case ValidationStatus.WrongLength:
                    throw new ArgumentException("Access key must be 40 in length", nameof(accessKey));
                default:
                    throw new ArgumentException("Failed to validate access key");
            }
        }

        public static bool TryValidateObjectKey(string objectKey, Level mode, out ValidationStatus status)
        {
            if (string.IsNullOrEmpty(objectKey))
            {
                status = ValidationStatus.NullInput;
                return false;
            }

            if (objectKey.Length > 1024)
            {
                status = ValidationStatus.WrongLength;
                return false;
            }

            if (mode == Level.Level0)
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

                if (mode == Level.Level3)
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }

                if (CharHelper.OneOf(c, '&', '$', '@', '=', ';', ':', '+', ' ', ',', '?'))
                    continue;

                if (CharHelper.InRange(c, (char)0, (char)31) || c == (char)127)
                    continue;

                if (mode == Level.Level2)
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }

                if (CharHelper.OneOf(c, '\\', '{', '}', '^', '%', '`', '[', ']', '"', '<', '>', '~', '#', '|'))
                    continue;

                if (CharHelper.InRange(c, (char)128, (char)255))
                    continue;

                if (mode == Level.Level1)
                {
                    status = ValidationStatus.WrongFormat;
                    return false;
                }
            }

            status = ValidationStatus.Ok;
            return true;
        }

        public static void ValidateObjectKey(string bucketName, Level mode)
        {
            Validator.RequireNotNull(bucketName, nameof(bucketName));

            if (TryValidateObjectKey(bucketName, mode, out ValidationStatus status))
                return;

            switch (status)
            {
                case ValidationStatus.WrongLength:
                    throw new ArgumentException("Object keys must be less than 1024 characters in length", nameof(bucketName));
                case ValidationStatus.WrongFormat:
                    throw new ArgumentException("Invalid character in object key. Only a-z, A-Z, 0-9 and !, -, _, ., *, ', ( and ) are allowed", nameof(bucketName));
                default:
                    throw new ArgumentException("Failed to validate key id");
            }
        }

        /// <summary>
        /// Validates a bucket name according to standard DNS rules. See https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html#bucketnamingrules for more details.
        /// </summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="status">Contains the error if validation failed</param>
        /// <returns>True if validation succeeded, false otherwise</returns>
        public static bool TryValidateBucketName(string bucketName, out ValidationStatus status)
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

        public static void ValidateBucketName(string bucketName)
        {
            Validator.RequireNotNull(bucketName, nameof(bucketName));

            if (TryValidateBucketName(bucketName, out ValidationStatus status))
                return;

            switch (status)
            {
                case ValidationStatus.WrongLength:
                    throw new ArgumentException("Bucket names must be less than 64 in length", nameof(bucketName));
                case ValidationStatus.WrongFormat:
                    throw new ArgumentException("Invalid character in object key. Only a-z, 0-9, . and - are allowed", nameof(bucketName));
                default:
                    throw new ArgumentException("Failed to validate key id");
            }
        }
    }
}