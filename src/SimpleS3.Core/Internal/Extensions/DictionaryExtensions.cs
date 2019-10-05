using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;

namespace Genbox.SimpleS3.Core.Internal.Extensions
{
    internal static class DictionaryExtensions
    {
        public static string GetHeader(this IDictionary<string, string> response, string key)
        {
            return TryGetHeader(response, key, out string value) ? value : null;
        }

        public static bool TryGetHeader(this IDictionary<string, string> response, string key, out string value)
        {
            if (response.TryGetValue(key, out value))
                return true;

            return false;
        }

        public static T GetHeaderEnum<T>(this IDictionary<string, string> response, string key) where T : struct, Enum
        {
            return TryGetHeader(response, key, out string value) ? ValueHelper.ParseEnum<T>(value) : default;
        }

        public static int GetHeaderInt(this IDictionary<string, string> response, string key)
        {
            return TryGetHeader(response, key, out string value) ? ValueHelper.ParseInt(value) : default;
        }

        public static long GetHeaderLong(this IDictionary<string, string> response, string key)
        {
            return TryGetHeader(response, key, out string value) ? ValueHelper.ParseLong(value) : default;
        }

        public static DateTimeOffset GetHeaderDate(this IDictionary<string, string> response, string key, DateTimeFormat format)
        {
            return TryGetHeader(response, key, out string value) ? ValueHelper.ParseDate(value, format) : default;
        }

        public static byte[] GetHeaderByteArray(this IDictionary<string, string> response, string key, BinaryEncoding encoding)
        {
            return TryGetHeader(response, key, out string value) ? ValueHelper.ParseByteArray(value, encoding) : default;
        }

        public static bool GetHeaderBool(this IDictionary<string, string> response, string key)
        {
            return TryGetHeader(response, key, out string value) ? ValueHelper.ParseBool(value) : default;
        }
    }
}