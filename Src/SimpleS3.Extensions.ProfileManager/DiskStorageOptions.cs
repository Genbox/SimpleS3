using System.Runtime.InteropServices;

namespace Genbox.SimpleS3.Extensions.ProfileManager;

/// <summary>Provides the <c>DiskStorageOptions</c> API.</summary>
public class DiskStorageOptions
{
    /// <summary>Provides the <c>DefaultLocation</c> API.</summary>
    public static string DefaultLocation => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleS3", "Profiles") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SimpleS3", "Profiles");

    /// <summary>Provides the <c>ProfileLocation</c> API.</summary>
    public string ProfileLocation { get; set; } = DefaultLocation;

    /// <summary>Provides the <c>OverwriteExisting</c> API.</summary>
    public bool OverwriteExisting { get; set; }
}