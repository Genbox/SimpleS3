using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal.Storage;

internal class DiskStorage : IStorage
{
    private readonly DiskStorageOptions _options;

    public DiskStorage(IOptions<DiskStorageOptions> options)
    {
        _options = options.Value;
    }

    public byte[]? Get(string name)
    {
        string path = Path.Combine(_options.ProfileLocation, name);

        if (!File.Exists(path))
            return null;

        return File.ReadAllBytes(path);
    }

    public string Put(string name, byte[] data)
    {
        string path = Path.Combine(_options.ProfileLocation, name);

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

        return Directory.EnumerateFiles(_options.ProfileLocation);
    }
}