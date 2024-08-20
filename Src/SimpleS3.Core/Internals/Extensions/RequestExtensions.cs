using System.Globalization;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Misc;

namespace Genbox.SimpleS3.Core.Internals.Extensions;

internal static class RequestExtensions
{
    public static void SetOptionalHeader(this IRequest request, string key, string? value)
    {
        if (value == null)
            return;

        request.SetHeader(key, value);
    }

    public static void SetHeader(this IRequest request, string key, bool? value, string falseLabel = "OFF", string trueLabel = "ON")
    {
        if (value == null)
            return;

        request.SetHeader(key, value.Value ? trueLabel : falseLabel);
    }

    public static void SetHeader(this IRequest request, string key, DateTimeOffset? date, DateTimeFormat format)
    {
        if (date == null)
            return;

        switch (format)
        {
            case DateTimeFormat.Iso8601DateTime:
                request.SetHeader(key, date.Value.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
                break;
            case DateTimeFormat.Iso8601DateTimeExt:
                request.SetHeader(key, date.Value.ToString(DateTimeFormats.Iso8601DateTimeExtended, DateTimeFormatInfo.InvariantInfo));
                break;
            case DateTimeFormat.Rfc1123:
                request.SetHeader(key, date.Value.ToString(DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo));
                break;
            case DateTimeFormat.Unknown:
            case DateTimeFormat.Iso8601Date:
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static void SetHeader(this IRequest request, string key, byte[]? data, BinaryEncoding encoding)
    {
        if (data == null)
            return;

        switch (encoding)
        {
            case BinaryEncoding.Base64:
                request.SetHeader(key, Convert.ToBase64String(data));
                break;
            case BinaryEncoding.Hex:
                request.SetHeader(key, data.HexEncode());
                break;
            case BinaryEncoding.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
        }
    }

    public static void SetHeader<T>(this IRequest request, string key, T value) where T : struct, Enum
    {
        if (value.Equals(default(T)))
            return;

        request.SetHeader(key, ValueHelper.EnumToString(value));
    }

    public static void SetHeader(this IRequest request, string key, long? value)
    {
        if (value == null)
            return;

        request.SetHeader(key, ValueHelper.LongToString(value.Value));
    }

    public static void SetHeader(this IRequest request, string key, IHttpHeaderBuilder? builder)
    {
        string? value = builder?.Build();

        if (value == null)
            return;

        request.SetHeader(key, value);
    }

    public static void SetOptionalQueryParameter(this IRequest request, string key, string? value)
    {
        if (value == null)
            return;

        request.SetQueryParameter(key, value);
    }

    public static void SetQueryParameter(this IRequest request, string key, DateTimeOffset? date, DateTimeFormat format)
    {
        if (date == null)
            return;

        switch (format)
        {
            case DateTimeFormat.Iso8601DateTime:
                request.SetQueryParameter(key, date.Value.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
                break;
            case DateTimeFormat.Rfc1123:
                request.SetQueryParameter(key, date.Value.ToString(DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo));
                break;
            case DateTimeFormat.Unknown:
            case DateTimeFormat.Iso8601Date:
            case DateTimeFormat.Iso8601DateTimeExt:
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static void SetQueryParameter(this IRequest request, string key, int? value)
    {
        if (value == null)
            return;

        request.SetQueryParameter(key, ValueHelper.IntToString(value.Value));
    }

    public static void SetQueryParameter<T>(this IRequest request, string key, T value) where T : struct, Enum
    {
        if (value.Equals(default(T)))
            return;

        request.SetQueryParameter(key, ValueHelper.EnumToString(value));
    }

    public static void SetQueryParameter(this IRequest request, string key, bool? value)
    {
        if (value == null)
            return;

        request.SetQueryParameter(key, ValueHelper.BoolToString(value.Value));
    }

    public static void SetQueryParameter(this IRequest request, string key, IHttpHeaderBuilder? builder)
    {
        string? value = builder?.Build();

        if (value == null)
            return;

        request.SetQueryParameter(key, value);
    }
}