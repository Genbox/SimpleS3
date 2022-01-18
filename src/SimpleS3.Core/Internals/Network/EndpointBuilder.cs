using System;
using System.Text.RegularExpressions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Common.Validation;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network
{
    internal class EndpointBuilder : IEndpointBuilder
    {
        private readonly Config _config;
        private readonly Regex _regex = new Regex("{(?:(?<pre>[^:}]*?):)?(?<val>Region|Bucket|Scheme)(?::(?<post>[^}]*?))?}", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public EndpointBuilder(IOptions<Config> config)
        {
            _config = config.Value;
        }

        public IEndpointData GetEndpoint(IRequest request)
        {
            Validator.RequireNotNull(_config.EndpointTemplate, nameof(Config.EndpointTemplate), "Unable to determine endpoint because both Endpoint and EndpointTemplate was null");

            string endpoint = _regex.Replace(_config.EndpointTemplate, match =>
            {
                string? str;

                string value = match.Groups["val"].Value.ToLowerInvariant();

                if (value == "bucket" && request is IHasBucketName bucketRequest)
                    str = bucketRequest.BucketName;
                else if (value == "scheme")
                    str = _config.UseTls ? "https" : "http";
                else if (value == "region")
                    str = _config.RegionCode;
                else
                    return null;

                string pre = match.Groups["pre"].Value;
                string post = match.Groups["post"].Value;
                return pre + str + post;
            });

            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri parsed))
                throw new InvalidOperationException(nameof(Config.EndpointTemplate) + " was invalid.");

            return new EndpointData(parsed.Host, parsed.AbsoluteUri.TrimEnd('/'));
        }
    }
}