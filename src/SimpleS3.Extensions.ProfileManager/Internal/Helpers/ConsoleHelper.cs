using System;
using System.Collections.Generic;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal.Helpers;

internal static class ConsoleHelper
{
    /// <summary>
    /// Securely reads a secret from console. Not only does it print out '*' for each entry, it also reads it into a char[], which can be cleared,
    /// which strings cannot.
    /// </summary>
    public static char[] ReadSecret(int expectedLength = 12)
    {
        List<char> pass = new List<char>(expectedLength);

        do
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Escape)
            {
                pass.Add(key.KeyChar);
                Console.Write("*");
            }
            else
            {
                if (key.Key == ConsoleKey.Backspace && pass.Count > 0)
                {
                    pass.RemoveAt(pass.Count - 1);
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    while (pass.Count > 0)
                    {
                        pass.RemoveAt(pass.Count - 1);
                        Console.Write("\b \b");
                    }
                }
            }
        } while (true);

        return pass.ToArray();
    }
}