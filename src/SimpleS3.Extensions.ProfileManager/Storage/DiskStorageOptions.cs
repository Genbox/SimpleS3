using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Storage
{
    public class DiskStorageOptions
    {
        public static string DefaultLocation => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleS3", "Profiles") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SimpleS3", "Profiles");

        public string ProfileLocation { get; set; } = DefaultLocation;
    }
}