using System.Globalization;
using System.Text.RegularExpressions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Common.Validation;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal class EndpointBuilder(IOptions<SimpleS3Config> config) : IEndpointBuilder
{
    private readonly SimpleS3Config _config = config.Value;
    private readonly Regex _regex = new Regex("{(?:(?<pre>[^:}]*?):)?(?<val>Region|Bucket|Scheme)(?::(?<post>[^}]*?))?}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private EndpointData? _lastCache;

    public IEndpointData GetEndpoint(IRequest request)
    {
        string? bucket = null;

        if (request is IHasBucketName bucketRequest)
            bucket = bucketRequest.BucketName;

        //We want to avoid all the string manipulation GetEndpointData does, so we cache the result here. However, if anything changed, we need to recalculate it.
        //We don't check on endpoint as it can be a template and therefore will never match.
        if (_lastCache != null && _lastCache.Bucket == bucket && _lastCache.RegionCode == _config.RegionCode)
            return _lastCache;

        _lastCache = GetEndpointData(bucket, _config.RegionCode);
        return _lastCache;
    }

    private EndpointData GetEndpointData(string? bucket, string regionCode)
    {
        string endpoint = _config.Endpoint;

        if (endpoint.IndexOf('{') >= 0)
        {
            endpoint = _regex.Replace(_config.Endpoint, match =>
            {
                string? str;

                string value = match.Groups["val"].Value.ToLowerInvariant();

                //We only replace bucket if there is one and the naming mode is VirtualHost
                if (value == "bucket" && bucket != null && _config.NamingMode == NamingMode.VirtualHost)
                    str = bucket;
                else if (value == "scheme")
                    str = _config.UseTls ? "https" : "http";
                else if (value == "region")
                    str = _config.RegionCode;
                else
                    return null!;

                string pre = match.Groups["pre"].Value;
                string post = match.Groups["post"].Value;
                return pre + str + post;
            });
        }

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? parsed))
            throw new InvalidOperationException(nameof(SimpleS3Config.Endpoint) + " is an invalid url: " + endpoint);

        //We need to add port for services hosted on non-standard ports. See #61
        string host = parsed.Host;

        if (!parsed.IsDefaultPort)
            host += ':' + parsed.Port.ToString(NumberFormatInfo.InvariantInfo);

        return new EndpointData(host, bucket, regionCode, parsed.AbsoluteUri.TrimEnd('/'));
    }
}