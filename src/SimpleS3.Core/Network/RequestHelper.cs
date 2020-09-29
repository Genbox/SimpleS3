using System;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network
{
    public static class RequestHelper
    {
        public static string BuildFullUrl<TReq>(S3Config config, TReq request) where TReq : IRequest
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent(200);
            AppendScheme(sb, config);
            AppendHost(sb, config, request);
            AppendUrl(sb, config, request);
            AppendQueryParameters(sb,request);

            string url = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return url;
        }

        public static void AppendScheme(StringBuilder sb, S3Config config)
        {
            if (config.Endpoint == null)
                sb.Append(config.UseTLS ? "https" : "http");
            else
                sb.Append(config.Endpoint.Scheme);

            sb.Append("://");
        }

        public static void AppendHost<TReq>(StringBuilder sb, S3Config config, TReq request) where TReq : IRequest
        {
            string? bucketName = null;

            if (request is IHasBucketName bn)
                bucketName = bn.BucketName;

            Uri? endpoint = config.Endpoint;

            if (endpoint != null)
            {
                sb.Append(endpoint.Host);

                if (!endpoint.IsDefaultPort)
                    sb.Append(':').Append(endpoint.Port);
            }
            else if (bucketName != null && config.NamingMode == NamingMode.VirtualHost)
                sb.Append(bucketName).Append(".s3.").Append(ValueHelper.EnumToString(config.Region)).Append(".amazonaws.com");
            else
                sb.Append("s3.").Append(ValueHelper.EnumToString(config.Region)).Append(".amazonaws.com");
        }

        public static void AppendUrl<TReq>(StringBuilder sb, S3Config config, TReq request) where TReq : IRequest
        {
            sb.Append('/');

            if (config.NamingMode == NamingMode.PathStyle && request is IHasBucketName bn)
                sb.Append(bn.BucketName).Append('/');

            if (request is IHasObjectKey ok)
                sb.Append(UrlHelper.UrlPathEncode(ok.ObjectKey));
        }

        public static void AppendQueryParameters<TReq>(StringBuilder sb, TReq request) where TReq : IRequest
        {
            if (request.QueryParameters.Count > 0)
                sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));
        }
    }
}