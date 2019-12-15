using System;
using System.Globalization;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Internals.Constants;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Internals.Extensions
{
    internal static class RequestExtensions
    {
        public static void AddHeader(this IRequest request, string key, bool? value, string falseLabel = "OFF", string trueLabel = "ON")
        {
            if (value == null)
                return;

            request.AddHeader(key, value.Value ? trueLabel : falseLabel);
        }

        public static void AddHeader(this IRequest request, string key, DateTimeOffset? date, DateTimeFormat format)
        {
            if (date == null)
                return;

            switch (format)
            {
                case DateTimeFormat.Iso8601DateTime:
                    request.AddHeader(key, date.Value.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
                    break;
                case DateTimeFormat.Iso8601DateTimeExt:
                    request.AddHeader(key, date.Value.ToString(DateTimeFormats.Iso8601DateTimeExtended, DateTimeFormatInfo.InvariantInfo));
                    break;
                case DateTimeFormat.Rfc1123:
                    request.AddHeader(key, date.Value.ToString(DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static void AddHeader(this IRequest request, string key, byte[] data, BinaryEncoding encoding)
        {
            if (data == null)
                return;

            switch (encoding)
            {
                case BinaryEncoding.Base64:
                    request.AddHeader(key, Convert.ToBase64String(data));
                    break;
                case BinaryEncoding.Hex:
                    request.AddHeader(key, data.HexEncode());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }
        }

        public static void AddHeader<T>(this IRequest request, string key, T value) where T : struct, Enum
        {
            if (value.Equals(default(T)))
                return;

            request.AddHeader(key, ValueHelper.EnumToString(value));
        }

        public static void AddHeader(this IRequest request, string key, long? value)
        {
            if (value == null)
                return;

            request.AddHeader(key, ValueHelper.LongToString(value.Value));
        }

        public static void AddHeader(this IRequest request, string key, IHttpHeaderBuilder builder)
        {
            if (builder == null)
                return;

            request.AddHeader(key, builder.Build());
        }

        public static void AddQueryParameter(this IRequest request, string key, DateTimeOffset? date, DateTimeFormat format)
        {
            if (date == null)
                return;

            switch (format)
            {
                case DateTimeFormat.Iso8601DateTime:
                    request.AddQueryParameter(key, date.Value.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo));
                    break;
                case DateTimeFormat.Rfc1123:
                    request.AddQueryParameter(key, date.Value.ToString(DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static void AddQueryParameter(this IRequest request, string key, int? value)
        {
            if (value == null)
                return;

            request.AddQueryParameter(key, ValueHelper.IntToString(value.Value));
        }

        public static void AddQueryParameter<T>(this IRequest request, string key, T value) where T : struct, Enum
        {
            if (value.Equals(default(T)))
                return;

            request.AddQueryParameter(key, ValueHelper.EnumToString(value));
        }

        public static void AddQueryParameter(this IRequest request, string key, bool? value)
        {
            if (value == null)
                return;

            request.AddQueryParameter(key, ValueHelper.BoolToString(value.Value));
        }

        public static void AddQueryParameter(this IRequest request, string key, IHttpHeaderBuilder builder)
        {
            if (builder == null)
                return;

            request.AddQueryParameter(key, builder.Build());
        }
    }
}