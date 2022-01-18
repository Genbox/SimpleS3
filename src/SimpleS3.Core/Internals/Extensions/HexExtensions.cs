using System.Globalization;

namespace Genbox.SimpleS3.Core.Internals.Extensions;

internal static class HexExtensions
{
    private static readonly uint[] Lookup32 = CreateLookup32();

    private static uint[] CreateLookup32()
    {
        uint[] result = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            string s = i.ToString("x2", CultureInfo.InvariantCulture);
            result[i] = s[0] + ((uint)s[1] << 16);
        }

        return result;
    }

    public static byte[] HexDecode(this string hex)
    {
        if (hex.Length == 0)
            return Array.Empty<byte>();

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            unchecked
            {
                int hi = hex[i * 2] - 65;
                hi = hi + 10 + ((hi >> 31) & 7);

                int lo = hex[i * 2 + 1] - 65;
                lo = (lo + 10 + ((lo >> 31) & 7)) & 0x0f;

                bytes[i] = (byte)(lo | (hi << 4));
            }
        }

        return bytes;
    }

    public static string HexEncode(this byte[] bytes)
    {
        if (bytes.Length == 0)
            return string.Empty;

        char[] result = new char[bytes.Length * 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            uint val = Lookup32[bytes[i]];
            unchecked
            {
                result[2 * i] = (char)val;
            }

            result[2 * i + 1] = (char)(val >> 16);
        }

        return new string(result);
    }
}