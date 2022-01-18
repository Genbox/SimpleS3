using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal class ScopeBuilder : IScopeBuilder
{
    private readonly IOptions<SimpleS3Config> _options;

    public ScopeBuilder(IOptions<SimpleS3Config> options)
    {
        _options = options;
    }

    public string CreateScope(string service, DateTimeOffset date)
    {
        return $"{ValueHelper.DateToString(date, DateTimeFormat.Iso8601Date)}/{_options.Value.RegionCode}/{service}/aws4_request";
    }
}