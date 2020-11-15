using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager
{
    public class Profile : IProfile
    {
        internal Profile() { }

        public IDictionary<string, string>? Tags { get; private set; }

        public string Name { get; internal set; }

        public string KeyId { get; internal set; }

        public byte[] AccessKey { get; internal set; }

        public string RegionCode { get; internal set; }

        public string Location { get; internal set; }

        public DateTimeOffset CreatedOn { get; internal set; }

        public void AddTag(string key, string value)
        {
            if (Tags == null)
                Tags = new Dictionary<string, string>(1);

            Tags.Add(key, value);
        }

        public string? GetTag(string key)
        {
            if (Tags == null)
                return null;

            return Tags.TryGetValue(key, out string value) ? value : null;
        }
    }
}