using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public sealed class ProfileManagerTests : IDisposable
{
    private readonly string _profileLocation = Path.Combine(Path.GetTempPath(), "SimpleS3-" + Guid.NewGuid());

    [Fact]
    public void UseProfileManagerBuildsWithValidationWithoutConsoleSetup()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INetworkDriver, NullNetworkDriver>();
        SimpleS3CoreServices.AddSimpleS3Core(services)
                            .UseProfileManager(options => options.ProfileLocation = _profileLocation);

        using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });

        Assert.NotNull(provider.GetRequiredService<IProfileManager>());
    }

    [Fact]
    public void UseProfileManagerPreservesCustomStorageAndSerializer()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<IStorage, CustomStorage>();
        services.AddSingleton<IProfileSerializer, CustomProfileSerializer>();

        SimpleS3CoreServices.AddSimpleS3Core(services)
                            .UseProfileManager(options => options.ProfileLocation = _profileLocation);

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.IsType<CustomStorage>(provider.GetRequiredService<IStorage>());
        Assert.IsType<CustomProfileSerializer>(provider.GetRequiredService<IProfileSerializer>());
    }

    [Fact]
    public void UseProfileManagerProtectsPersistedAccessKeyByDefault()
    {
        using ServiceProvider provider = CreateProvider();
        IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();

        byte[] originalSecret = "plain-secret-key"u8.ToArray();
        profileManager.CreateProfile("profile", "key-id", originalSecret, "region", out string? location);
        Assert.NotNull(location);

        string persistedProfile = Encoding.UTF8.GetString(File.ReadAllBytes(location));
        Assert.DoesNotContain(Convert.ToBase64String(originalSecret), persistedProfile, StringComparison.Ordinal);
        Assert.DoesNotContain("plain-secret-key", persistedProfile, StringComparison.Ordinal);
    }

    [Fact]
    public void GetProfileMigratesLegacyUnprotectedProfile()
    {
        Directory.CreateDirectory(_profileLocation);
        byte[] originalSecret = "legacy-secret-key"u8.ToArray();
        string profilePath = Path.Combine(_profileLocation, "legacy");

        // Version 1
        File.WriteAllText(profilePath, "Name=" + "legacy" + Environment.NewLine +
                                       "CreatedOn=" + DateTimeOffset.UtcNow.UtcDateTime.ToString("O") + Environment.NewLine +
                                       "Location=" + Environment.NewLine +
                                       "KeyId=key-id" + Environment.NewLine +
                                       "AccessKey=" + Convert.ToBase64String(originalSecret) + Environment.NewLine +
                                       "RegionCode=region" + Environment.NewLine, Encoding.UTF8);

        using ServiceProvider provider = CreateProvider();
        IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();
        IAccessKeyProtector protector = provider.GetRequiredService<IAccessKeyProtector>();

        IProfile? profile = profileManager.GetProfile("legacy");
        Assert.NotNull(profile);
        string migratedProfile = File.ReadAllText(profilePath, Encoding.UTF8);

        Assert.Equal(2, profile.ProfileVersion);
        Assert.Equal(originalSecret, protector.UnprotectKey(profile.AccessKey));
        Assert.Contains("ProfileVersion=2", migratedProfile, StringComparison.Ordinal);
        Assert.Contains("Tag.Protector=DataProtectionKeyProtector", migratedProfile, StringComparison.Ordinal);
        Assert.DoesNotContain(Convert.ToBase64String(originalSecret), migratedProfile, StringComparison.Ordinal);
        Assert.DoesNotContain("legacy-secret-key", migratedProfile, StringComparison.Ordinal);
    }

    [Fact]
    public void GetProfileStampsLegacyProtectedProfileWithoutReprotectingKey()
    {
        using ServiceProvider provider = CreateProvider();
        IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();
        profileManager.CreateProfile("protected", "key-id", "protected-secret-key"u8.ToArray(), "region", out string? location);
        Assert.NotNull(location);

        string protectedProfile = RemoveProfileVersion(File.ReadAllText(location, Encoding.UTF8));
        string protectedAccessKey = GetProfileValue(protectedProfile, nameof(IProfile.AccessKey));
        File.WriteAllText(location, protectedProfile, Encoding.UTF8);

        IProfile? profile = profileManager.GetProfile("protected");
        Assert.NotNull(profile);
        string migratedProfile = File.ReadAllText(location, Encoding.UTF8);

        Assert.Equal(2, profile.ProfileVersion);
        Assert.Equal(protectedAccessKey, GetProfileValue(migratedProfile, nameof(IProfile.AccessKey)));
        Assert.Contains("ProfileVersion=2", migratedProfile, StringComparison.Ordinal);
    }

    public void Dispose()
    {
        if (Directory.Exists(_profileLocation))
            Directory.Delete(_profileLocation, true);
    }

    private ServiceProvider CreateProvider()
    {
        ServiceCollection services = new ServiceCollection();
        SimpleS3CoreServices.AddSimpleS3Core(services)
                            .UseProfileManager(options => options.ProfileLocation = _profileLocation);
        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(_profileLocation, "keys")));

        return services.BuildServiceProvider();
    }

    private static string RemoveProfileVersion(string profileText)
    {
        string[] lines = profileText.Split([Environment.NewLine], StringSplitOptions.None);
        return string.Join(Environment.NewLine, lines.Where(line => !line.StartsWith("ProfileVersion=", StringComparison.Ordinal)));
    }

    private static string GetProfileValue(string profileText, string key)
    {
        foreach (string line in profileText.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith(key + '=', StringComparison.Ordinal))
                return line[(key.Length + 1)..];
        }

        throw new InvalidOperationException("Missing profile key: " + key);
    }

    private sealed class CustomStorage : IStorage
    {
        public byte[]? Get(string name) => throw new NotSupportedException();
        public string Put(string name, byte[] data, bool forceOverwrite = false) => throw new NotSupportedException();
        public IEnumerable<string> List() => throw new NotSupportedException();
    }

    private sealed class CustomProfileSerializer : IProfileSerializer
    {
        public byte[] Serialize<T>(T profile) where T : IProfile => throw new NotSupportedException();
        public T Deserialize<T>(byte[] data) where T : IProfile => throw new NotSupportedException();
    }
}