using System;
using Genbox.SimpleS3.Utils.Enums;

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
    }
}
