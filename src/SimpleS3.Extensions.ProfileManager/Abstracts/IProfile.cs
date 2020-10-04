using System;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts
{
    public interface IProfile
    {
        string Name { get; }
        string KeyId { get; }
        byte[] AccessKey { get; }
        string RegionCode { get; }
        string Location { get; }
        DateTimeOffset CreatedOn { get; }
        void AddTag(string key, string value);
        string? GetTag(string key);
    }
}