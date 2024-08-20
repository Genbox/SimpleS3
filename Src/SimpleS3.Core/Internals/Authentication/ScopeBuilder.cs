using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed class ScopeBuilder(IOptions<SimpleS3Config> options) : IScopeBuilder
{
    private readonly SimpleS3Config _options = options.Value;

    public string CreateScope(string service, DateTimeOffset date) => $"{ValueHelper.DateToString(date, DateTimeFormat.Iso8601Date)}/{_options.RegionCode}/{service}/aws4_request";
}