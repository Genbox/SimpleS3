using System;

namespace Genbox.SimpleS3.Core.Internals.Helpers
{
    internal static class CopyHelper
    {
        public static byte[]? CopyWithNull(byte[]? input)
        {
            if (input == null)
                return null;

            byte[] arr = new byte[input.Length];
            Array.Copy(input, 0, arr, 0, input.Length);
            return arr;
        }
    }
}