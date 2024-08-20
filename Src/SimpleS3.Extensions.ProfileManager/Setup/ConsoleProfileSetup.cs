using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.Helpers;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Setup;

public class ConsoleProfileSetup(IProfileManager profileManager, IInputValidator inputValidator, IRegionConverter regionConverter, IRegionData regionData) : IProfileSetup
{
    public IProfile SetupProfile(string profileName, bool persist = true)
    {
        IProfile? existingProfile = profileManager.GetProfile(profileName);

        if (existingProfile != null)
            return existingProfile;

        ConsoleKey key;
        string enteredKeyId;
        byte[] accessKey;
        IRegionInfo region;

        do
        {
            enteredKeyId = GetKeyId();
            accessKey = GetAccessKey();
            region = GetRegion();

            Console.WriteLine();
            Console.WriteLine("Please confirm the following information:");
            Console.WriteLine("Key id: " + enteredKeyId);
            Console.WriteLine("Region: " + region.Code + " -- " + region.Name);
            Console.WriteLine();

            do
            {
                Console.WriteLine("Is it correct? Y/N");

                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.Y && key != ConsoleKey.N);
        } while (key == ConsoleKey.N);

        IProfile profile = profileManager.CreateProfile(profileName, enteredKeyId, accessKey, region.Code, persist);

        if (persist)
        {
            if (!string.IsNullOrEmpty(profile.Location))
                Console.WriteLine("Successfully saved the profile to " + profile.Location);
            else
                Console.WriteLine("Successfully saved profile");
        }

        //Clear the access key from memory
        Array.Clear(accessKey, 0, accessKey.Length);

        return profile;
    }

    private string GetKeyId()
    {
        string? enteredKeyId;

        Console.WriteLine();
        Console.WriteLine("Enter your key id - Example: AKIAIOSFODNN7EXAMPLE");

        while (true)
        {
            enteredKeyId = Console.ReadLine();

            if (inputValidator.TryValidateKeyId(enteredKeyId, out _, out _))
                break;

            Console.Error.WriteLine("Invalid key id. Try again.");
        }

        Validator.RequireNotNull(enteredKeyId);

        return enteredKeyId.Trim();
    }

    private byte[] GetAccessKey()
    {
        char[]? enteredAccessKey;
        byte[]? utf8AccessKey;

        Console.WriteLine();
        Console.WriteLine("Enter your access key - Example: wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");

        while (true)
        {
            enteredAccessKey = ConsoleHelper.ReadSecret(40);

            //Now we trim any whitespace characters the user might have entered
            int end;
            int start = 0;

            while (start < enteredAccessKey.Length)
            {
                if (!char.IsWhiteSpace(enteredAccessKey[start]))
                    break;

                start++;
            }

            for (end = enteredAccessKey.Length - 1; end >= start; end--)
            {
                if (!char.IsWhiteSpace(enteredAccessKey[end]))
                    break;
            }

            if (start != 0 || end != enteredAccessKey.Length - 1)
            {
                char[] trimmed = new char[end + 1 - start];

                int count = 0;
                for (int i = start; i < end + 1; i++, count++)
                    trimmed[count] = enteredAccessKey[i];

                Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);

                enteredAccessKey = trimmed;
            }

            utf8AccessKey = Encoding.UTF8.GetBytes(enteredAccessKey);

            if (inputValidator.TryValidateAccessKey(utf8AccessKey, out _, out _))
                break;

            Console.Error.WriteLine("Invalid access key. Try again.");
            Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);
            Array.Clear(utf8AccessKey, 0, utf8AccessKey.Length);
        }

        //Clear the access key from memory
        Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);

        return utf8AccessKey;
    }

    private IRegionInfo GetRegion()
    {
        Console.WriteLine();
        Console.WriteLine("Choose the default region. You can choose it by index or region code");

        HashSet<string> validRegionId = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int counter = 0; //used for validation further down

        Console.WriteLine("{0,-8}{1,-20}{2}", "Index", "Region Code", "Region Name");
        foreach (IRegionInfo regionInfo in regionData.GetRegions())
        {
            validRegionId.Add(regionInfo.Code);
            Console.WriteLine("{0,-8}{1,-20}{2}", Convert.ChangeType(regionInfo.EnumValue, typeof(int), NumberFormatInfo.InvariantInfo), regionInfo.Code, regionInfo.Name);

            counter++;
        }

        while (true)
        {
            string? enteredRegion = Console.ReadLine();

            if (enteredRegion != null)
            {
                if (int.TryParse(enteredRegion, out int index) && index >= 0 && index <= counter)
                    return regionConverter.GetRegion(index);

                if (validRegionId.Contains(enteredRegion))
                    return regionConverter.GetRegion(enteredRegion);
            }

            Console.Error.WriteLine("Invalid region. Try again.");
        }
    }
}