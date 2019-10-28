using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Cli.Core.Managers;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Cli
{
    public class CliManager
    {
        private static CliManager _cliManager;

        private CliManager(string profileName, AwsRegion region)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSimpleS3WithProfile(profileName, config =>
            {
                if (region != AwsRegion.Unknown)
                    config.Region = region;
            })
            .UseDataProtection();

            services.AddSingleton<BucketManager>();
            services.AddSingleton<ObjectManager>();

            ServiceProvider provider = services.BuildServiceProvider();
            ProfileManager = provider.GetRequiredService<IProfileManager>();

            IProfile profile = ProfileManager.GetProfile(profileName) ?? ConsoleSetup.SetupProfile(ProfileManager, profileName);

            S3Client = provider.GetRequiredService<IS3Client>();
            BucketManager = provider.GetRequiredService<BucketManager>();
            ObjectManager = provider.GetRequiredService<ObjectManager>();
        }

        private IS3Client S3Client { get; }
        public BucketManager BucketManager { get; }
        public ObjectManager ObjectManager { get; }
        public IProfileManager ProfileManager { get; }

        public static CliManager GetCliManager(string profile, AwsRegion region)
        {
            return _cliManager ?? new CliManager(profile ?? Extensions.ProfileManager.ProfileManager.DefaultProfile, region);
        }
    }
}