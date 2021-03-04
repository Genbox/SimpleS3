using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager
{
    public class ProfileManager : IProfileManager
    {
        public const string DefaultProfile = "DefaultProfile";
        private readonly IOptions<ProfileManagerOptions> _options;
        private readonly IAccessKeyProtector? _protector;
        private readonly IProfileSerializer _serializer;
        private readonly IStorage _storage;

        private readonly IInputValidator _validator;

        public ProfileManager(IInputValidator validator, IProfileSerializer serializer, IStorage storage, IOptions<ProfileManagerOptions> options, IAccessKeyProtector? protector = null)
        {
            _validator = validator;
            _serializer = serializer;
            _storage = storage;
            _options = options;
            _protector = protector;
        }

        public IProfile? GetProfile(string name)
        {
            byte[]? data;

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
            string? protector = profile.GetTag("Protector");

            string profileProtector = protector ?? string.Empty;
            string userProtector = _protector != null ? _protector.GetType().Name : string.Empty;

            if (!string.Equals(profileProtector, userProtector, StringComparison.OrdinalIgnoreCase))
                throw new S3Exception("The access key is protected with " + protector + " but it was not available");

            return profile;
        }

        public IProfile CreateProfile(string name, string keyId, byte[] accessKey, string region, bool persist = true)
        {
            _validator.ValidateKeyId(keyId);
            _validator.ValidateAccessKey(accessKey);

            Profile profile = new Profile();
            profile.Name = name;
            profile.KeyId = keyId;
            profile.AccessKey = KeyHelper.ProtectKey(accessKey, _protector, _options.Value.ClearInputKey);

            if (_protector != null)
                profile.AddTag("Protector", _protector.GetType().Name);

            profile.CreatedOn = DateTimeOffset.UtcNow;
            profile.RegionCode = region;

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