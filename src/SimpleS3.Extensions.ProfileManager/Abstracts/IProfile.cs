using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts
{
    public interface IProfile
    {
        string Name { get; }
        string KeyId { get; }
        byte[] AccessKey { get; }
        AwsRegion Region { get; }
        string Location { get; }
        DateTimeOffset CreatedOn { get; }
        void AddTag(string key, string value);
        string? GetTag(string key);
    }
}