using System.Globalization;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Misc;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class ValueHelper
{
    public static T ParseEnum<T>(string? value) where T : struct, Enum => EnumHelper.TryParse(value, out T parsedValue) ? parsedValue : default;

    public static int ParseInt(string? value) => int.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int result) ? result : 0;

    public static long ParseLong(string? value) => long.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out long result) ? result : 0;

    public static bool ParseBool(string? value) => bool.TryParse(value, out bool result) && result;

    public static DateTimeOffset ParseDate(string? value, DateTimeFormat format)
    {
        Validator.RequireNotNullOrWhiteSpace(value);

        return format switch
        {
            DateTimeFormat.Iso8601Date => DateTimeOffset.TryParseExact(value, DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset result) ? result : DateTimeOffset.MinValue,
            DateTimeFormat.Iso8601DateTime => DateTimeOffset.TryParseExact(value, DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset result) ? result : DateTimeOffset.MinValue,
            DateTimeFormat.Iso8601DateTimeExt => DateTimeOffset.TryParseExact(value, DateTimeFormats.Iso8601DateTimeExtended, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset result) ? result : DateTimeOffset.MinValue,
            DateTimeFormat.Rfc1123 => DateTimeOffset.TryParseExact(value, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset result) ? result : DateTimeOffset.MinValue,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static byte[]? ParseByteArray(string? value, BinaryEncoding encoding)
    {
        if (value == null)
            return null;

        return encoding switch
        {
            BinaryEncoding.Hex => value.HexDecode(),
            BinaryEncoding.Base64 => Convert.FromBase64String(value),
            _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null)
        };
    }

    public static string EnumToString<T>(T value) where T : struct, Enum
    {
        string? str = EnumHelper.AsString(value);

        if (str == null)
            throw new S3Exception("Unable to parse enum");

        return str;
    }

    public static string BoolToString(bool value) => value.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();

    public static string IntToString(int value) => value.ToString(NumberFormatInfo.InvariantInfo);

    public static string LongToString(long value) => value.ToString(NumberFormatInfo.InvariantInfo);

    public static string DateToString(DateTimeOffset date, DateTimeFormat format)
    {
        return format switch
        {
            DateTimeFormat.Iso8601Date => date.ToString(DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo),
            DateTimeFormat.Iso8601DateTime => date.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo),
            DateTimeFormat.Iso8601DateTimeExt => date.ToString(DateTimeFormats.Iso8601DateTimeExtended, DateTimeFormatInfo.InvariantInfo),
            DateTimeFormat.Rfc1123 => date.ToString(DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}