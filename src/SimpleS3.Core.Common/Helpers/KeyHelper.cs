using System;
using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class KeyHelper
{
    public static byte[] ProtectKey(byte[] key, IAccessKeyProtector? protector, bool clearKey = true)
    {
        if (protector == null)
            return key;

        byte[] previous = key;
        key = protector.ProtectKey(key);

        if (clearKey)
            Array.Clear(previous, 0, previous.Length);

        return key;
    }

    public static byte[] UnprotectKey(byte[] key, IAccessKeyProtector? protector)
    {
        if (protector == null)
            return key;

        return protector.UnprotectKey(key);
    }
}