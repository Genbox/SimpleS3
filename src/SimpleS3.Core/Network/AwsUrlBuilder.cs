using System;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network
{
    public class AwsUrlBuilder : IUrlBuilder
    {
        private readonly IOptions<Config> _options;

        public AwsUrlBuilder(IOptions<Config> options)
        {
            _options = options;
        }

        public void AppendHost<TReq>(StringBuilder sb, TReq request) where TReq : IRequest
        {
            string? bucketName = null;

            if (request is IHasBucketName bn)
                bucketName = bn.BucketName;

            Config config = _options.Value;

            Uri? endpoint = config.Endpoint;

            if (endpoint != null)
            {
                sb.Append(endpoint.Host);

                if (!endpoint.IsDefaultPort)
                    sb.Append(':').Append(endpoint.Port);
            }
            else if (bucketName != null && config.NamingMode == NamingMode.VirtualHost)
                sb.Append(bucketName).Append(".s3.").Append(config.RegionCode).Append(".amazonaws.com");
            else
                sb.Append("s3.").Append(config.RegionCode).Append(".amazonaws.com");
        }

        public void AppendUrl<TReq>(StringBuilder sb, TReq request) where TReq : IRequest
        {
            sb.Append('/');

            Config config = _options.Value;

            if (config.NamingMode == NamingMode.PathStyle && request is IHasBucketName bn)
                sb.Append(bn.BucketName).Append('/');

            if (request is IHasObjectKey ok)
                sb.Append(UrlHelper.UrlPathEncode(ok.ObjectKey));
        }
    }
}