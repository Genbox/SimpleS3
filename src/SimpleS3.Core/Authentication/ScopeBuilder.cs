using System;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Authentication
{
    public class ScopeBuilder : IScopeBuilder
    {
        private readonly IOptions<AwsConfig> _options;

        public ScopeBuilder(IOptions<AwsConfig> options)
        {
            _options = options;
        }

        public string CreateScope(string service, DateTimeOffset date)
        {
            return $"{ValueHelper.DateToString(date, DateTimeFormat.Iso8601Date)}/{ValueHelper.EnumToString(_options.Value.Region)}/{service}/aws4_request";
        }
    }
}