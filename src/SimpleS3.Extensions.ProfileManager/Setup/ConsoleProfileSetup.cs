using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.Helpers;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Setup
{
    public class ConsoleProfileSetup : IProfileSetup
    {
        private readonly IProfileManager _profileManager;
        private readonly IInputValidator _inputValidator;
        private readonly IRegionManager _regionManager;

        public ConsoleProfileSetup(IProfileManager profileManager, IInputValidator inputValidator, IRegionManager regionManager)
        {
            _profileManager = profileManager;
            _inputValidator = inputValidator;
            _regionManager = regionManager;
        }

        public IProfile SetupProfile(string profileName, bool persist = true)
        {
            IProfile? existingProfile = _profileManager.GetProfile(profileName);

            if (existingProfile != null)
                return existingProfile;

            start:

            string enteredKeyId = GetKeyId();
            byte[] accessKey = GetAccessKey();
            IRegionInfo region = GetRegion();

            Console.WriteLine();
            Console.WriteLine("Please confirm the following information:");
            Console.WriteLine("Key id: " + enteredKeyId);
            Console.WriteLine("Region: " + region.Code + " -- " + region.Name);
            Console.WriteLine();

            ConsoleKey key;

            do
            {
                Console.WriteLine("Is it correct? Y/N");

                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.Y && key != ConsoleKey.N);

            if (key == ConsoleKey.N)
                goto start;

            IProfile profile = _profileManager.CreateProfile(profileName, enteredKeyId, accessKey, region.Code, persist);

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
            bool validKeyId = true;

            Console.WriteLine();
            Console.WriteLine("Enter your key id - Example: AKIAIOSFODNN7EXAMPLE");

            do
            {
                if (!validKeyId)
                    Console.Error.WriteLine("Invalid key id. Try again.");

                enteredKeyId = Console.ReadLine();

                if (!string.IsNullOrEmpty(enteredKeyId))
                    enteredKeyId = enteredKeyId.Trim();
            } while (!(validKeyId = _inputValidator.TryValidateKeyId(enteredKeyId, out _)));

            return enteredKeyId!;
        }

        private byte[] GetAccessKey()
        {
            char[]? enteredAccessKey = null;
            byte[]? utf8AccessKey = null;
            bool validAccessKey = true;

            Console.WriteLine();
            Console.WriteLine("Enter your access key - Example: wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");

            do
            {
                if (!validAccessKey)
                {
                    Console.Error.WriteLine("Invalid access key. Try again.");
                    Array.Clear(enteredAccessKey!, 0, enteredAccessKey!.Length);
                    Array.Clear(utf8AccessKey!, 0, utf8AccessKey!.Length);
                }

                enteredAccessKey = ConsoleHelper.ReadSecret(40);

                //Now we trim any whitespace characters the user might have entered

                int end;
                int start;

                for (start = 0; start < enteredAccessKey.Length;)
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
                    {
                        trimmed[count] = enteredAccessKey[i];
                    }

                    Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);

                    enteredAccessKey = trimmed;
                }

                utf8AccessKey = Encoding.UTF8.GetBytes(enteredAccessKey);
            } while (!(validAccessKey = _inputValidator.TryValidateAccessKey(utf8AccessKey, out _)));

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
            foreach (IRegionInfo regionInfo in _regionManager.GetAllRegions())
            {
                validRegionId.Add(regionInfo.Code);
                Console.WriteLine("{0,-8}{1,-20}{2}", Convert.ChangeType(regionInfo.EnumValue, typeof(int), NumberFormatInfo.InvariantInfo), regionInfo.Code, regionInfo.Name);

                counter++;
            }

        start2:
            string? enteredRegion = Console.ReadLine();

            if (enteredRegion != null)
            {
                if (int.TryParse(enteredRegion, out int index) && index >= 0 && index <= counter)
                    return _regionManager.GetRegionInfo(index);

                if (validRegionId.Contains(enteredRegion))
                    return _regionManager.GetRegionInfo(enteredRegion);
            }

            Console.Error.WriteLine("Invalid region. Try again.");
            goto start2;
        }
    }
}