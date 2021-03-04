using System;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Marshal;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Common
{
    public abstract class UrlBuilderBase : IUrlBuilder
    {
        private readonly IOptions<Config> _options;

        protected UrlBuilderBase(IOptions<Config> options)
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

            if (endpoint == null)
            {
                if (config.NamingMode == NamingMode.VirtualHost)
                    AppendVirtualHost(sb, config, bucketName);
                else
                    AppendPathStyle(sb, config, bucketName);
            }
            else
            {
                sb.Append(endpoint.Host);

                if (!endpoint.IsDefaultPort)
                    sb.Append(':').Append(endpoint.Port);
            }
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

        protected abstract void AppendVirtualHost(StringBuilder sb, Config config, string? bucketName);
        protected abstract void AppendPathStyle(StringBuilder sb, Config config, string? bucketName);
    }
}