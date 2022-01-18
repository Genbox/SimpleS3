using System;
using Genbox.SimpleS3.Core.Common.Internals;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class EnumHelper
{
    public static string AsString<T>(T value) where T : Enum
    {
        return EnumCache<T>.Instance.AsString(ref value);
    }

    public static bool TryParse<T>(string? value, out T enumVal) where T : struct, Enum
    {
        if (value == null)
        {
            enumVal = default;
            return false;
        }

        return Enum.TryParse(value, true, out enumVal) || EnumCache<T>.Instance.TryGetValueFromString(value, out enumVal);
    }
}