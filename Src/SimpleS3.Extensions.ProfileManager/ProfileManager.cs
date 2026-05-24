using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager;

public class ProfileManager(IInputValidator validator, IProfileSerializer serializer, IStorage storage, IOptions<ProfileManagerOptions> options, IAccessKeyProtector? protector = null) : IProfileManager
{
    public const string DefaultProfile = "DefaultProfile";
    private const int _currentProfileVersion = 2;
    private readonly ProfileManagerOptions _config = options.Value;

    public IProfile? GetProfile(string name)
    {
        byte[]? data;

        try
        {
            data = storage.Get(name);
        }
        catch
        {
            return null;
        }

        if (data == null)
            return null;

        Profile profile = serializer.Deserialize<Profile>(data);

        if (profile.ProfileVersion < _currentProfileVersion)
            MigrateFromVersion1(name, profile);

        //Check if the we have the right protector
        string? protector1 = profile.GetTag("Protector");

        string profileProtector = protector1 ?? string.Empty;
        string userProtector = protector != null ? protector.GetType().Name : string.Empty;

        if (!string.Equals(profileProtector, userProtector, StringComparison.OrdinalIgnoreCase))
            throw new S3Exception($"The access key is protected with '{protector1}' but it was not available");

        return profile;
    }

    public IProfile CreateProfile(string name, string keyId, byte[] accessKey, string region, out string? location, bool persist = true)
    {
        validator.ValidateKeyIdAndThrow(keyId);
        validator.ValidateAccessKeyAndThrow(accessKey);

        Profile profile = new Profile();
        profile.Name = name;
        profile.KeyId = keyId;
        profile.AccessKey = KeyHelper.ProtectKey(accessKey, protector, _config.ClearInputKey);
        profile.ProfileVersion = _currentProfileVersion;

        if (protector != null)
            profile.AddTag("Protector", protector.GetType().Name);

        profile.CreatedOn = DateTimeOffset.UtcNow;
        profile.RegionCode = region;

        if (persist)
        {
            byte[] data = serializer.Serialize(profile);
            location = storage.Put(profile.Name, data);
        }
        else
            location = null;

        return profile;
    }

    public IEnumerable<IProfile> List()
    {
        foreach (string name in storage.List())
        {
            IProfile? profile = GetProfile(name);

            if (profile == null)
                throw new InvalidOperationException("Concurrency: A profile was deleted white iterating them");

            yield return profile;
        }
    }

    public string SaveProfile(IProfile profile)
    {
        byte[] data = serializer.Serialize(profile);
        return storage.Put(profile.Name, data);
    }

    private void MigrateFromVersion1(string name, Profile profile)
    {
        string? profileProtector = profile.GetTag("Protector");

        if (profileProtector == null && protector != null)
        {
            profile.AccessKey = KeyHelper.ProtectKey(profile.AccessKey, protector, _config.ClearInputKey);
            profile.AddTag("Protector", protector.GetType().Name);
        }

        profile.ProfileVersion = _currentProfileVersion;
        byte[] data = serializer.Serialize(profile);
        storage.Put(name, data, true);
    }
}