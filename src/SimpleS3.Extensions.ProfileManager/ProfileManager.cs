using System;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager
{
    public class ProfileManager : IProfileManager
    {
        public const string DefaultProfile = "DefaultProfile";
        public const AwsRegion DefaultRegion = AwsRegion.UsEast1;
        private readonly IOptions<ProfileManagerOptions> _options;
        private readonly IAccessKeyProtector _protector;

        private readonly IProfileSerializer _serializer;
        private readonly IStorage _storage;

        public ProfileManager(IProfileSerializer serializer, IStorage storage, IOptions<ProfileManagerOptions> options, IAccessKeyProtector protector = null)
        {
            _serializer = serializer;
            _storage = storage;
            _options = options;
            _protector = protector;
        }

        public IProfile GetProfile(string name)
        {
            byte[] data;

            try
            {
                data = _storage.Get(name);
            }
            catch
            {
                return null;
            }

            if (data == null)
                return null;

            IProfile profile = _serializer.Deserialize<Profile>(data);

            //Check if the we have the right protector
            string protector = profile.GetTag("Protector");

            if (!string.IsNullOrEmpty(protector))
            {
                if (_protector == null || !protector.Equals(_protector.GetType().Name, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("The access key is protected with " + protector + " but it was not available");
            }

            return profile;
        }

        public IProfile CreateProfile(string name, string keyId, byte[] accessKey, AwsRegion region = DefaultRegion, bool persist = true)
        {
            InputValidator.ValidateKeyId(keyId);
            InputValidator.ValidateAccessKey(accessKey);

            Profile profile = new Profile();
            profile.Name = name;
            profile.KeyId = keyId;
            profile.AccessKey = KeyHelper.ProtectKey(accessKey, _protector, _options.Value.ClearInputKey);

            if (_protector != null)
                profile.AddTag("Protector", _protector.GetType().Name);

            profile.CreatedOn = DateTimeOffset.UtcNow;
            profile.Region = region;

            if (persist)
                profile.Location = SaveProfile(profile);

            return profile;
        }

        public string SaveProfile(IProfile profile)
        {
            byte[] data = _serializer.Serialize(profile);
            return _storage.Put(profile.Name, data);
        }
    }
}