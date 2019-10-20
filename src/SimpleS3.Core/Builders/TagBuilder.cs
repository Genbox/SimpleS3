using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Builders
{
    public class TagBuilder : IHttpHeaderBuilder
    {
        private readonly Regex _validChar = new Regex(@"^[a-z0-9 \+\-=\._:/]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private IDictionary<string, string> _tags;

        public string Build()
        {
            if (_tags == null)
                return null;

            return string.Join("&", _tags.Select(x => x.Key + '=' + x.Value));
        }

        public string HeaderName => null;

        //From https://docs.aws.amazon.com/awsaccountbilling/latest/aboutv2/allocation-tag-restrictions.html
        // - The maximum key length is 128 Unicode characters.
        // - The maximum value length is 256 Unicode characters.
        // - Tags are case sensitive.
        // - The maximum number of tags per resource is 50.
        // - The maximum active tag keys for Billing and Cost Management reports is 500.
        // - The reserved prefix is aws:.
        // - AWS generated tag names and values are automatically assigned the aws: prefix, which you can't assign. User-defined tag names have the prefix user: in the cost allocation report.
        // - Use each key only once for each resource. If you attempt to use the same key twice on the same resource, your request will be rejected.
        // - In some services, you can tag a resource when you create it. For more information, see the documentation for the service where you want to tag resources.
        // - You can't backdate the application of a tag. This means that tags only start appearing on your cost allocation report after you apply them and don't appear on earlier reports.
        // - Allowed characters are Unicode letters, whitespace, and numbers, plus the following special characters: + - = . _ : /

        public TagBuilder Add(string key, string value)
        {
            Validator.RequireNotNull(key, nameof(key));
            Validator.RequireNotNull(value, nameof(value));

            if (_tags == null)
                _tags = new Dictionary<string, string>();

            if (_tags.Count == 50)
                throw new Exception("Only 50 tags allowed per. object");

            if (key.Length > 128)
                throw new ArgumentException("Keys can only be up to 128 characters", nameof(key));

            if (!_validChar.IsMatch(key))
                throw new ArgumentException("Keys must be made of letters, whitespace, and numbers, plus the following special characters: + - = . _ : /", nameof(key));

            if (value.Length > 256)
                throw new ArgumentException("Values can only be up to 256 characters", nameof(value));

            if (!_validChar.IsMatch(value))
                throw new ArgumentException("Values must be made of letters, whitespace, and numbers, plus the following special characters: + - = . _ : /", nameof(key));

            _tags.Add(key, value);

            return this;
        }
    }
}