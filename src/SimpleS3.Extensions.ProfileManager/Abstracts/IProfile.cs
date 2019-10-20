using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Abstracts.Enums;

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
        IDictionary<string, string> Tags { get; }

        void AddTag(string key, string value);
        string GetTag(string key);
    }
}