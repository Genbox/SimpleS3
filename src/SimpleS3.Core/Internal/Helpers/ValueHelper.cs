using System;
using System.Globalization;
using EnumsNET;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;

namespace Genbox.SimpleS3.Core.Internal.Helpers
{
    internal static class ValueHelper
    {
        public static T ParseEnum<T>(string value) where T : struct, Enum
        {
            return EnumsNET.Enums.TryParse(value, true, out T parsedValue, EnumFormat.EnumMemberValue, EnumFormat.Name, EnumFormat.UnderlyingValue) ? parsedValue : default;
        }

        public static int ParseInt(string value)
        {
            return int.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int result) ? result : 0;
        }

        public static long ParseLong(string value)
        {
            return long.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out long result) ? result : 0;
        }

        public static bool ParseBool(string value)
        {
            return bool.TryParse(value, out bool result) && result;
        }

        public static DateTimeOffset ParseDate(string value, DateTimeFormat format)
        {
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

        public static byte[] ParseByteArray(string value, BinaryEncoding encoding)
        {
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
            return value.AsString(EnumFormat.EnumMemberValue, EnumFormat.Name);
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