namespace Genbox.SimpleS3.Core.Common.Extensions;

public static class StringExtensions
{
    public static bool Contains(this string str, string value, StringComparison comparison) => str.IndexOf(value, comparison) >= 0;

    public static bool Contains(this string str, char value) => str.IndexOf(value) >= 0;

    public static bool EndsWith(this string str, char character)
    {
        if (str.Length == 0)
            return false;

        return str[str.Length - 1] == character;
    }
}