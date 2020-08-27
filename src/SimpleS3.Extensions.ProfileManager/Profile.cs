using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager
{
    public class Profile : IProfile
    {
        private IDictionary<string, string>? _tags; 

        internal Profile()
        {
        }

        public string Name { get; internal set; }

        public string KeyId { get; internal set; }

        public byte[] AccessKey { get; internal set; }

        public AwsRegion Region { get; internal set; }

        public string Location { get; internal set; }

        public DateTimeOffset CreatedOn { get; internal set; }

        public void AddTag(string key, string value)
        {
            if (_tags == null)
                _tags = new Dictionary<string, string>(1);

            _tags.Add(key, value);
        }

        public string? GetTag(string key)
        {
            if (_tags == null)
                return null;

            return _tags.TryGetValue(key, out string value) ? value : null;
        }
    }
}