using System.Text.RegularExpressions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Common.Validation;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal class EndpointBuilder : IEndpointBuilder
{
    private readonly SimpleS3Config _config;
    private readonly Regex _regex = new Regex("{(?:(?<pre>[^:}]*?):)?(?<val>Region|Bucket|Scheme)(?::(?<post>[^}]*?))?}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private EndpointData? _lastCache;

    public EndpointBuilder(IOptions<SimpleS3Config> config)
    {
        _config = config.Value;
    }

    public IEndpointData GetEndpoint(IRequest request)
    {
        string? bucket = null;

        if (request is IHasBucketName bucketRequest)
            bucket = bucketRequest.BucketName;

        //Bucket is set
        if (_lastCache != null && _lastCache.Bucket == bucket && _lastCache.RegionCode == _config.RegionCode)
            return _lastCache;

        _lastCache = GetEndpointData(bucket, _config.RegionCode);
        return _lastCache;
    }

    private EndpointData GetEndpointData(string? bucket, string regionCode)
    {
        string? endpoint = _config.Endpoint?.ToString();

        if (endpoint == null)
        {
            Validator.RequireNotNull(_config.EndpointTemplate, "Unable to determine endpoint because both Endpoint and EndpointTemplate was null");

            endpoint = _regex.Replace(_config.EndpointTemplate, match =>
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
            throw new InvalidOperationException(nameof(SimpleS3Config.EndpointTemplate) + " was resolved to an invalid url: " + endpoint);

        return new EndpointData(parsed.Host, bucket, regionCode, parsed.AbsoluteUri.TrimEnd('/'));
    }
}