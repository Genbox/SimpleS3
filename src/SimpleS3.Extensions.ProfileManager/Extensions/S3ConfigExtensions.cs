using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions
{
    public static class S3ConfigExtensions
    {
        public static void UseProfile(this IS3Config config, IProfileManager profileManager, string profileName = ProfileManager.DefaultProfile)
        {
            IProfile profile = profileManager.GetProfile(profileName);

            if (profile == null)
                throw new Exception("The profile " + profileName + " does not exist.");

            config.Credentials = new AccessKey(profile.KeyId, profile.AccessKey);
            config.Region = profile.Region;
        }
    }
}