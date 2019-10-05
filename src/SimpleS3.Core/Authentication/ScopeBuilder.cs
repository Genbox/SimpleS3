using System;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Authentication
{
    public class ScopeBuilder : IScopeBuilder
    {
        private readonly IOptions<S3Config> _options;

        public ScopeBuilder(IOptions<S3Config> options)
        {
            _options = options;
        }

        public string CreateScope(string service, DateTimeOffset date)
        {
            return $"{ValueHelper.DateToString(date, DateTimeFormat.Iso8601Date)}/{ValueHelper.EnumToString(_options.Value.Region)}/{service}/aws4_request";
        }
    }
}