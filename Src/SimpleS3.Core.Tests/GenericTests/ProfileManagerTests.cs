using System.Text;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public sealed class ProfileManagerTests : IDisposable
{
    private readonly string _profileLocation = Path.Combine(Path.GetTempPath(), "SimpleS3-" + Guid.NewGuid());

    [Fact]
    public void UseProfileManagerProtectsPersistedAccessKeyByDefault()
    {
        ServiceCollection services = new ServiceCollection();
        SimpleS3CoreServices.AddSimpleS3Core(services)
                            .UseProfileManager(options => options.ProfileLocation = _profileLocation);
        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(_profileLocation, "keys")));

        using ServiceProvider provider = services.BuildServiceProvider();
        IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();

        byte[] originalSecret = "plain-secret-key"u8.ToArray();
        profileManager.CreateProfile("profile", "key-id", originalSecret, "region", out string? location);
        Assert.NotNull(location);

        string persistedProfile = Encoding.UTF8.GetString(File.ReadAllBytes(location));
        Assert.DoesNotContain(Convert.ToBase64String(originalSecret), persistedProfile, StringComparison.Ordinal);
        Assert.DoesNotContain("plain-secret-key", persistedProfile, StringComparison.Ordinal);
    }

    public void Dispose()
    {
        if (Directory.Exists(_profileLocation))
            Directory.Delete(_profileLocation, true);
    }
}