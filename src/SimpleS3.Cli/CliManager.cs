using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Cli.Core.Managers;
using Genbox.SimpleS3.Extensions.ProfileManager;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Serializers;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Genbox.SimpleS3.Extensions.ProfileManager.Storage;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Cli
{
    public class CliManager
    {
        private static CliManager _cliManager;

        private CliManager(string profileName, AwsRegion region)
        {
            JsonProfileSerializer serializer = new JsonProfileSerializer();
            DiskStorage storage = new DiskStorage(Options.Create(new DiskStorageOptions()));
            ProfileManager = new ProfileManager(serializer, storage, Options.Create(new ProfileManagerOptions()));

            IProfile profile = ProfileManager.GetProfile(profileName) ?? ConsoleSetup.SetupProfile(ProfileManager, profileName);

            S3Client s3Client = new S3Client(profile.KeyId, profile.AccessKey, region != AwsRegion.Unknown ? region : profile.Region);
            BucketManager = new BucketManager(s3Client);
        }

        public BucketManager BucketManager { get; }
        public IProfileManager ProfileManager { get; }

        public static CliManager GetCliManager(string profile, AwsRegion region)
        {
            return _cliManager ?? new CliManager(profile, region);
        }
    }
}