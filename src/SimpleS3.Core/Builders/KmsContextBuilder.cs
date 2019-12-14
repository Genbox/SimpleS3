using System;
using System.Collections.Generic;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Builders
{
    public class KmsContextBuilder : IHttpHeaderBuilder
    {
        private IDictionary<string, string> _dict;

        public KmsContextBuilder(IDictionary<string, string> dict = null)
        {
            if (dict == null)
                return;

            foreach (KeyValuePair<string, string> pair in dict)
                AddEntry(pair.Key, pair.Value);
        }

        public string Build()
        {
            if (!HasData())
                return null;

            return JsonHelper.EncodeJson(_dict);
        }

        public void Reset()
        {
            _dict?.Clear();
        }

        public bool HasData()
        {
            return _dict != null && _dict.Count > 0;
        }

        public string HeaderName => null;

        public void AddEntry(string key, string value)
        {
            Validator.RequireNotNull(key, nameof(key));
            Validator.RequireNotNull(value, nameof(value));

            if (_dict == null)
                _dict = new Dictionary<string, string>();

            if (!Valid(key))
                throw new ArgumentException("Invalid character in key. Only digits, letters and the follow characters are allowed _, -, /, \\, :", nameof(key));

            if (!Valid(value))
                throw new ArgumentException("Invalid character in value. Only digits, letters and the follow characters are allowed _, -, /, \\, :", nameof(value));

            _dict.Add(key, value);
        }

        private static bool Valid(string item)
        {
            foreach (char c in item)
            {
                if (c == '_' || c == '-' || c == '/' || c == '\\' || c == ':')
                    continue;

                if (char.IsLetterOrDigit(c))
                    continue;

                return false;
            }

            return true;
        }
    }
}