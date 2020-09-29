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
        public static string BuildHost<TReq>(S3Config config, TReq request) where TReq : IRequest
        {
            string? bucketName = null;

            if (request is IHasBucketName bn)
                bucketName = bn.BucketName;

            StringBuilder sb = StringBuilderPool.Shared.Rent(100);

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

            string host = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return host;
        }

        public static string BuildUrl<TReq>(string host, S3Config config, TReq request) where TReq : IRequest
        {
            string? bucketName = null;

            if (request is IHasBucketName bn)
                bucketName = bn.BucketName;

            string? objectKey = null;

            if (request is IHasObjectKey ok)
                objectKey = ok.ObjectKey;

            StringBuilder sb = StringBuilderPool.Shared.Rent(100);

            if (config.Endpoint == null)
                sb.Append(config.UseTLS ? "https" : "http");
            else
                sb.Append(config.Endpoint.Scheme);

            sb.Append("://");
            sb.Append(host);
            sb.Append('/');

            if (bucketName != null && config.NamingMode == NamingMode.PathStyle)
                sb.Append(bucketName).Append('/');

            //Ensure that the object key is encoded
            if (objectKey != null)
                sb.Append(UrlHelper.UrlPathEncode(objectKey));

            //Map all the parameters on to the url
            if (request.QueryParameters.Count > 0)
                sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));

            string url = sb.ToString();
            StringBuilderPool.Shared.Return(sb);
            return url;
        }
    }
}