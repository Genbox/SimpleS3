using System.Runtime.CompilerServices;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class CharHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InRange(char c, char min, char max)
    {
        return unchecked((uint)(c - min) <= (uint)(max - min));
    }

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