using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Internals.Extensions;

internal static class DictionaryExtensions
{
    public static string? GetOptionalValue(this IDictionary<string, string> response, string key) => TryGetHeader(response, key, out string? value) ? value : null;

    public static string GetRequiredValue(this IDictionary<string, string> response, string key)
    {
        if (response.TryGetValue(key, out string? value))
            return value;

        throw new S3Exception($"Failed to get required header '{key}'");
    }

    public static bool TryGetHeader(this IDictionary<string, string> response, string key, [NotNullWhen(true)]out string? value)
    {
        return response.TryGetValue(key, out value);
    }

    public static int GetHeaderInt(this IDictionary<string, string> response, string key) => TryGetHeader(response, key, out string? value) ? ValueHelper.ParseInt(value) : default;

    public static long GetHeaderLong(this IDictionary<string, string> response, string key) => TryGetHeader(response, key, out string? value) ? ValueHelper.ParseLong(value) : default;

    public static DateTimeOffset GetHeaderDate(this IDictionary<string, string> response, string key, DateTimeFormat format) => TryGetHeader(response, key, out string? value) ? ValueHelper.ParseDate(value, format) : default;

    public static byte[]? GetHeaderByteArray(this IDictionary<string, string> response, string key, BinaryEncoding encoding) => TryGetHeader(response, key, out string? value) ? ValueHelper.ParseByteArray(value, encoding) : null;

    public static bool GetHeaderBool(this IDictionary<string, string> response, string key) => TryGetHeader(response, key, out string? value) && ValueHelper.ParseBool(value);
}