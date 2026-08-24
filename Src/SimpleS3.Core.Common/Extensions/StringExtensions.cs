namespace Genbox.SimpleS3.Core.Common.Extensions;

/// <summary>Provides the <c>StringExtensions</c> API.</summary>
public static class StringExtensions
{
    /// <summary>Provides the <c>Contains</c> API.</summary>
    public static bool Contains(this string str, string value, StringComparison comparison) => str.IndexOf(value, comparison) >= 0;

    /// <summary>Provides the <c>Contains</c> API.</summary>
    public static bool Contains(this string str, char value) => str.IndexOf(value) >= 0;

    /// <summary>Provides the <c>EndsWith</c> API.</summary>
    public static bool EndsWith(this string str, char character)
    {
        if (str.Length == 0)
            return false;

        return str[str.Length - 1] == character;
    }
}