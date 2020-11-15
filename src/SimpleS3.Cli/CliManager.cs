using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Cli.Core.Managers;
using Genbox.SimpleS3.Core.Aws;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Cli
{
    public class CliManager
    {
        private static CliManager _cliManager;

        private CliManager(string profileName, AwsRegion region)
        {
            ServiceCollection services = new ServiceCollection();
            IS3ClientBuilder builder = services.AddSimpleS3();

            builder.CoreBuilder.UseProfileManager()
                               .UseConsoleSetup()
                               .UseDataProtection();

            //Override the region if it is set via commandline
            builder.Services.PostConfigure<AwsConfig>(config =>
            {
                if (region != AwsRegion.Unknown)
                    config.Region = region;
            });

            services.AddSingleton<BucketManager>();
            services.AddSingleton<ObjectManager>();

            ServiceProvider provider = services.BuildServiceProvider();
            ProfileManager = provider.GetRequiredService<IProfileManager>();

            IProfileSetup? setup = provider.GetRequiredService<IProfileSetup>();
            setup.SetupProfile(profileName);

            S3Client = provider.GetRequiredService<IClient>();
            BucketManager = provider.GetRequiredService<BucketManager>();
            ObjectManager = provider.GetRequiredService<ObjectManager>();
        }

        private IClient S3Client { get; }
        public BucketManager BucketManager { get; }
        public ObjectManager ObjectManager { get; }
        public IProfileManager ProfileManager { get; }

        public static CliManager GetCliManager(string? profile, AwsRegion region)
        {
            return _cliManager ??= new CliManager(profile ?? Extensions.ProfileManager.ProfileManager.DefaultProfile, region);
        }
    }
}