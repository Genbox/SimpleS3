using System;
using System.Globalization;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Misc;

namespace Genbox.SimpleS3.Core.Internals.Helpers
{
    internal static class ValueHelper
    {
        public static T ParseEnum<T>(string? value) where T : struct, Enum
        {
            return EnumHelper.TryParse(value, out T parsedValue) ? parsedValue : default;
        }

        public static int ParseInt(string? value)
        {
            return int.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int result) ? result : 0;
        }

        public static long ParseLong(string? value)
        {
            return long.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out long result) ? result : 0;
        }

        public static bool ParseBool(string? value)
        {
            return bool.TryParse(value, out bool result) && result;
        }

        public static DateTimeOffset ParseDate(string? value, DateTimeFormat format)
        {
            Validator.RequireNotNull(value, nameof(value));

            switch (format)
            {
                case DateTimeFormat.Iso8601Date:
                    return DateTimeOffset.TryParseExact(value, DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset result) ? result : DateTimeOffset.MinValue;
                case DateTimeFormat.Iso8601DateTime:
                    return DateTimeOffset.TryParseExact(value, DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result) ? result : DateTimeOffset.MinValue;
                case DateTimeFormat.Iso8601DateTimeExt:
                    return DateTimeOffset.TryParseExact(value, DateTimeFormats.Iso8601DateTimeExtended, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result) ? result : DateTimeOffset.MinValue;
                case DateTimeFormat.Rfc1123:
                    return DateTimeOffset.TryParseExact(value, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result) ? result : DateTimeOffset.MinValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static byte[]? ParseByteArray(string? value, BinaryEncoding encoding)
        {
            if (value == null)
                return null;

            switch (encoding)
            {
                case BinaryEncoding.Hex:
                    return value.HexDecode();
                case BinaryEncoding.Base64:
                    return Convert.FromBase64String(value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }
        }

        public static string EnumToString<T>(T value) where T : struct, Enum
        {
            string? str = EnumHelper.AsString(value);

            if (str == null)
                throw new S3Exception("Unable to parse enum");

            return str;
        }

        public static string BoolToString(bool value)
        {
            return value.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
        }

        public static string IntToString(int value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        public static string LongToString(long value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        public static string DateToString(DateTimeOffset date, DateTimeFormat format)
        {
            switch (format)
            {
                case DateTimeFormat.Iso8601Date:
                    return date.ToString(DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo);
                case DateTimeFormat.Iso8601DateTime:
                    return date.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo);
                case DateTimeFormat.Iso8601DateTimeExt:
                    return date.ToString(DateTimeFormats.Iso8601DateTimeExtended, DateTimeFormatInfo.InvariantInfo);
                case DateTimeFormat.Rfc1123:
                    return date.ToString(DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}