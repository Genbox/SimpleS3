using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Storage;

public class DiskStorage(IOptions<DiskStorageOptions> options) : IStorage
{
    private readonly DiskStorageOptions _options = options.Value;

    public byte[]? Get(string name)
    {
        string path = GetProfilePath(name);

        if (!File.Exists(path))
            return null;

        return File.ReadAllBytes(path);
    }

    public string Put(string name, byte[] data)
    {
        string path = GetProfilePath(name);

        if (!Directory.Exists(_options.ProfileLocation))
            Directory.CreateDirectory(_options.ProfileLocation);

        if (File.Exists(path))
        {
            if (_options.OverwriteExisting)
                File.WriteAllBytes(path, data);
            else
                throw new S3Exception($"Cannot overwrite existing profile {name} because {nameof(DiskStorageOptions.OverwriteExisting)} is {_options.OverwriteExisting}");
        }
        else
            File.WriteAllBytes(path, data);

        return path;
    }

    public IEnumerable<string> List()
    {
        if (!Directory.Exists(_options.ProfileLocation))
            return Array.Empty<string>();

        return Directory.EnumerateFiles(_options.ProfileLocation).Select(x => Path.GetFileName(x)!);
    }

    private string GetProfilePath(string name)
    {
        if (Path.IsPathRooted(name) || name.IndexOf(Path.DirectorySeparatorChar) >= 0 || name.IndexOf(Path.AltDirectorySeparatorChar) >= 0 || name is "." or ".." || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new S3Exception("Profile name must be a safe file name");

        string root = Path.GetFullPath(_options.ProfileLocation);

        if (!root.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            root += Path.DirectorySeparatorChar;

        string path = Path.GetFullPath(Path.Combine(root, name));

        if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            throw new S3Exception("Profile name resolves outside the profile directory");

        return path;
    }
}