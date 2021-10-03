using System;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.Shared
{
    public static class UtilityHelper
    {
        public static S3Provider SelectProvider()
        {
            ConsoleKeyInfo key;
            int intVal = 0;
            S3Provider[] choices = Enum.GetValues<S3Provider>().Skip(1).ToArray();

            do
            {
                Console.WriteLine("Please select which provider you want to use:");

                foreach (S3Provider choice in choices)
                {
                    Console.WriteLine($"{(int)choice}. {choice}");
                }

                key = Console.ReadKey(true);
            } while (!choices.Any(x => int.TryParse(key.KeyChar.ToString(), out intVal) && (int)x == intVal));

            return (S3Provider)intVal;
        }

        public static string GetProfileName(S3Provider provider)
        {
            return "TestSetup-" + provider;
        }

        public static string GetTestBucket(IProfile profile)
        {
            string uniqId = profile.KeyId.Substring(0, 8);
            return "testbucket-" + uniqId;
        }

        public static ServiceProvider CreateSimpleS3(S3Provider selectedProvider, string profileName)
        {
            ServiceCollection services = new ServiceCollection();
            ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);

            coreBuilder.UseHttpClientFactory();

            coreBuilder.UseProfileManager()
                       .BindConfigToProfile(profileName)
                       .UseConsoleSetup();

            if (selectedProvider == S3Provider.AmazonS3)
                coreBuilder.UseAmazonS3();
            else if (selectedProvider == S3Provider.BackBlazeB2)
                coreBuilder.UseBackBlazeB2();
            else if (selectedProvider == S3Provider.GoogleCloudStorage)
                coreBuilder.UseGoogleCloudStorage();
            else
                throw new ArgumentOutOfRangeException(nameof(selectedProvider), selectedProvider, null);

            return services.BuildServiceProvider();
        }

        public static IProfile GetOrSetupProfile(IServiceProvider serviceProvider, S3Provider provider, string profileName)
        {
            //Check if there is a profile for this provider
            IProfileManager profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            IProfile? profile = profileManager.GetProfile(profileName);

            if (profile == null)
            {
                Console.WriteLine("The profile " + profileName + " does not exist.");

                if (provider == S3Provider.AmazonS3)
                    Console.WriteLine("You can create a new API key at https://console.aws.amazon.com/iam/home?#/security_credentials");
                else if (provider == S3Provider.BackBlazeB2)
                    Console.WriteLine("You can create a new API key at https://secure.backblaze.com/app_keys.htm");

                IProfileSetup profileSetup = serviceProvider.GetRequiredService<IProfileSetup>();
                return profileSetup.SetupProfile(profileName);
            }

            return profile;
        }
    }
}