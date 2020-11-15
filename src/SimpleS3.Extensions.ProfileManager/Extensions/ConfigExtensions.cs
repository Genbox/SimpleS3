using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions
{
    public static class ConfigExtensions
    {
        public static void UseProfile(this Config config, IProfileManager profileManager, string profileName)
        {
            IProfile? profile = profileManager.GetProfile(profileName);

            if (profile == null)
                throw new S3Exception("The profile " + profileName + " does not exist.");

            config.Credentials = new ProfileAccessKey(profile);
            config.RegionCode = profile.RegionCode;
        }

        public static void UseDefaultProfile(this Config config, IProfileManager profileManager)
        {
            UseProfile(config, profileManager, ProfileManager.DefaultProfile);
        }
    }
}