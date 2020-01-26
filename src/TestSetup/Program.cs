using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.TestSetup
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();

            ClientBuilder builder = new ClientBuilder(services);
            builder.UseProfileManager()
                .UseDataProtection();

            using (ServiceProvider provider = services.BuildServiceProvider())
            {
                IProfileManager manager = provider.GetRequiredService<IProfileManager>();
                IProfile profile = manager.GetDefaultProfile();

                //If profile is null, then we do not yet have a profile stored on disk. We use ConsoleSetup as an easy and secure way of asking for credentials
                if (profile == null)
                    ConsoleSetup.SetupDefaultProfile(manager);
                else
                    Console.WriteLine($"The profile {profile.Name} already exists. Nothing to do here.");
            }
        }
    }
}