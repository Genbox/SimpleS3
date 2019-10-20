using System.IO;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Storage
{
    public class DiskStorage : IStorage
    {
        private readonly IOptions<DiskStorageOptions> _options;

        public DiskStorage(IOptions<DiskStorageOptions> options)
        {
            _options = options;
        }

        public byte[] Get(string name)
        {
            string path = Path.Combine(_options.Value.ProfileLocation, name);

            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }

        public string Put(string name, byte[] data)
        {
            string path = Path.Combine(_options.Value.ProfileLocation, name);

            if (!Directory.Exists(_options.Value.ProfileLocation))
                Directory.CreateDirectory(_options.Value.ProfileLocation);

            File.WriteAllBytes(path, data);
            return path;
        }
    }
}