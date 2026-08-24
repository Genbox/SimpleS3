using System.Runtime.CompilerServices;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Provides the <c>CharHelper</c> API.</summary>
public static class CharHelper
{
    /// <summary>Provides the <c>InRange</c> API.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InRange(char c, char min, char max) => unchecked((uint)(c - min) <= (uint)(max - min));

    /// <summary>Provides the <c>OneOf</c> API.</summary>
    public static bool OneOf(char c, params char[] chars)
    {
        foreach (char ch in chars)
        {
            if (c == ch)
                return true;
        }

        return false;
    }
}