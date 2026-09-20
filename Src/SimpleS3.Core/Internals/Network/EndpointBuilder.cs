using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Marshal;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal class EndpointBuilder(IOptions<SimpleS3Config> config) : IEndpointBuilder
{
    private static readonly Regex _templateRegex = new Regex("{(?:(?<pre>[^:}]*?):)?(?<val>Region|Bucket|Scheme)(?::(?<post>[^}]*?))?}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private readonly SimpleS3Config _config = config.Value;
    private readonly ConcurrentDictionary<(string Endpoint, string? Bucket, string Region, string Provider, bool UseTls, NamingMode NamingMode), EndpointData> _endpointCache = new ConcurrentDictionary<(string Endpoint, string? Bucket, string Region, string Provider, bool UseTls, NamingMode NamingMode), EndpointData>();

    public IEndpointData GetEndpoint(IRequest request)
    {
        string? bucket = null;

        if (request is IHasBucketName bucketRequest)
            bucket = bucketRequest.BucketName;

        string endpoint = _config.Endpoint;
        string region = _config.RegionCode;
        string provider = _config.ProviderName;
        bool useTls = _config.UseTls;
        NamingMode namingMode = _config.NamingMode;

        (string Endpoint, string? Bucket, string Region, string Provider, bool UseTls, NamingMode NamingMode) cacheKey = (endpoint, bucket, region, provider, useTls, namingMode);

        if (_endpointCache.TryGetValue(cacheKey, out EndpointData? cached))
            return cached;

        EndpointData created = GetEndpointData(endpoint, bucket, region, provider, useTls, namingMode);

        // Keep the cache useful for alternating buckets without retaining an unbounded set of tenant-controlled names.
        if (_endpointCache.Count >= 256)
            _endpointCache.Clear();

        return _endpointCache.GetOrAdd(cacheKey, created);
    }

    private static EndpointData GetEndpointData(string endpoint, string? bucket, string regionCode, string provider, bool useTls, NamingMode namingMode)
    {
        // S3-012: AWS China regions use the amazonaws.com.cn DNS partition.
        if (provider == "AmazonS3" && regionCode.StartsWith("cn-", StringComparison.OrdinalIgnoreCase) && endpoint.EndsWith("amazonaws.com", StringComparison.Ordinal))
            endpoint += ".cn";

        if (endpoint.IndexOf('{') >= 0)
        {
            endpoint = _templateRegex.Replace(endpoint, match =>
            {
                string? str;

                string value = match.Groups["val"].Value.ToLowerInvariant();

                //We only replace bucket if there is one and the naming mode is VirtualHost
                if (value == "bucket" && bucket != null && namingMode == NamingMode.VirtualHost)
                    str = bucket;
                else if (value == "scheme")
                    str = useTls ? "https" : "http";
                else if (value == "region")
                    str = regionCode;
                else
                    return null!;

                string pre = match.Groups["pre"].Value;
                string post = match.Groups["post"].Value;
                return pre + str + post;
            });
        }

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? parsed))
            throw new InvalidOperationException(nameof(SimpleS3Config.Endpoint) + " is an invalid url: " + endpoint);

        if (parsed.Query.Length > 0 || parsed.Fragment.Length > 0)
            throw new InvalidOperationException(nameof(SimpleS3Config.Endpoint) + " cannot contain a query string or fragment: " + endpoint);

        //We need to add port for services hosted on non-standard ports. See #61
        string host = parsed.Host;

        if (!parsed.IsDefaultPort)
            host += ':' + parsed.Port.ToString(NumberFormatInfo.InvariantInfo);

        return new EndpointData(host, bucket, regionCode, parsed.AbsoluteUri.TrimEnd('/'));
    }
}