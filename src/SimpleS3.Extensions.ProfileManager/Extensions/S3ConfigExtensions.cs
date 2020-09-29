using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions
{
    public static class S3ConfigExtensions
    {
        public static void UseProfile(this IConfig config, IProfileManager profileManager, string profileName)
        {
            IProfile? profile = profileManager.GetProfile(profileName);

            if (profile == null)
                throw new Exception("The profile " + profileName + " does not exist.");

            config.Credentials = new AccessKey(profile.KeyId, profile.AccessKey);
            config.Region = profile.Region;
        }

        public static void UseDefaultProfile(this IConfig config, IProfileManager profileManager)
        {
            UseProfile(config, profileManager, ProfileManager.DefaultProfile);
        }
    }
}