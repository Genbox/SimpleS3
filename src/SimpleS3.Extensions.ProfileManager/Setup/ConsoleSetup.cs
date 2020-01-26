using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.Helpers;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Setup
{
    public static class ConsoleSetup
    {
        public static IProfile SetupDefaultProfile(IProfileManager profileManager, bool persist = true)
        {
            return SetupProfile(profileManager, ProfileManager.DefaultProfile, persist);
        }

        public static IProfile SetupProfile(IProfileManager profileManager, string profileName, bool persist = true)
        {
            start:
            Console.WriteLine();
            Console.WriteLine("You don't have a profile set up yet. Please enter your API credentials.");
            Console.WriteLine("You can create a new API key at https://console.aws.amazon.com/iam/home?#/security_credentials");

            string enteredKeyId = GetKeyId();
            byte[] accessKey = GetAccessKey();
            AwsRegion region = GetRegion();

            Console.WriteLine();
            Console.WriteLine("Please confirm the following information:");
            Console.WriteLine("Key id: " + enteredKeyId);
            Console.WriteLine("Region: " + GetEnumMember(region) + " -- " + region.GetRegionName());
            Console.WriteLine();

            ConsoleKey key;

            do
            {
                Console.WriteLine("Is it correct? Y/N");

                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.Y && key != ConsoleKey.N);

            if (key == ConsoleKey.N)
                goto start;

            IProfile profile = profileManager.CreateProfile(profileName, enteredKeyId, accessKey, region, persist);

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

        private static string GetKeyId()
        {
            string enteredKeyId;
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
            } while (!(validKeyId = InputValidator.TryValidateKeyId(enteredKeyId, out _)));

            return enteredKeyId;
        }

        private static byte[] GetAccessKey()
        {
            char[] enteredAccessKey = null;
            byte[] utf8AccessKey = null;
            bool validAccessKey = true;

            Console.WriteLine();
            Console.WriteLine("Enter your access key - Example: wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");

            do
            {
                if (!validAccessKey)
                {
                    Console.Error.WriteLine("Invalid access key. Try again.");
                    Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);
                    Array.Clear(utf8AccessKey, 0, utf8AccessKey.Length);
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
                        trimmed[count] = enteredAccessKey[i];

                    Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);

                    enteredAccessKey = trimmed;
                }

                utf8AccessKey = Encoding.UTF8.GetBytes(enteredAccessKey);
            } while (!(validAccessKey = InputValidator.TryValidateAccessKey(utf8AccessKey, out _)));

            //Clear the access key from memory
            Array.Clear(enteredAccessKey, 0, enteredAccessKey.Length);

            return utf8AccessKey;
        }

        private static AwsRegion GetRegion()
        {
            Console.WriteLine();
            Console.WriteLine("Choose the default region. You can choose it by index or region code");

            string[] names = Enum.GetNames(typeof(AwsRegion)).Skip(1).ToArray();

            Console.WriteLine("{0,-8}{1,-20}{2}", "Index", "Region Code", "Region Name");
            foreach (string name in names)
            {
                AwsRegion region = (AwsRegion)Enum.Parse(typeof(AwsRegion), name);
                Console.WriteLine("{0,-8}{1,-20}{2}", (int)region, GetEnumMember(region), region.GetRegionName());
            }

            string enteredRegion;
            bool validRegion = true;
            AwsRegion parsedRegion;

            do
            {
                if (!validRegion)
                    Console.Error.WriteLine("Invalid region. Try again.");

                enteredRegion = Console.ReadLine();
            } while (!(validRegion = Enum.TryParse(enteredRegion, out parsedRegion) && Enum.IsDefined(typeof(AwsRegion), parsedRegion)));

            return parsedRegion;
        }

        private static string GetEnumMember(AwsRegion region)
        {
            Type type = region.GetType();
            MemberInfo[] memInfo = type.GetMember(region.ToString());
            IEnumerable<EnumMemberAttribute> attributes = memInfo[0].GetCustomAttributes<EnumMemberAttribute>();
            return attributes.First().Value;
        }
    }
}