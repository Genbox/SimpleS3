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
        public static (string host, string url) BuildEndpointData<TReq>(S3Config config, TReq request) where TReq : IRequest
        {
            string? bucketName = null;

            if (request is IHasBucketName bn)
                bucketName = bn.BucketName;

            string? objectKey = null;

            if (request is IHasObjectKey ok)
                objectKey = ok.ObjectKey;

            //Ensure that the object key is encoded
            string? encodedResource = objectKey != null ? UrlHelper.UrlPathEncode(objectKey) : null;

            if (config.Endpoint == null || config.NamingMode == NamingMode.PathStyle)
            {
                if (bucketName != null)
                    objectKey = bucketName + '/' + encodedResource;
                else
                    objectKey = encodedResource;
            }
            else
                objectKey = encodedResource;

            StringBuilder sb = StringBuilderPool.Shared.Rent(100);

            Uri? endpoint = config.Endpoint;

            if (endpoint != null)
            {
                sb.Append(endpoint.Host);

                if (!endpoint.IsDefaultPort)
                    sb.Append(':').Append(endpoint.Port);
            }
            else if (config.NamingMode == NamingMode.VirtualHost)
            {
                if (bucketName != null)
                    sb.Append(bucketName).Append(".s3.").Append(ValueHelper.EnumToString(config.Region)).Append(".amazonaws.com");
                else
                    sb.Append("s3.").Append(ValueHelper.EnumToString(config.Region)).Append(".amazonaws.com");
            }
            else
                sb.Append("s3.").Append(ValueHelper.EnumToString(config.Region)).Append(".amazonaws.com");

            string host = sb.ToString();

            sb.Append('/').Append(objectKey);

            //Map all the parameters on to the url
            if (request.QueryParameters.Count > 0)
                sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));

            string fullUrl = (config.Endpoint == null ? config.UseTLS ? "https" : "http" : config.Endpoint.Scheme) + "://" + sb;

            StringBuilderPool.Shared.Return(sb);
            return (host, fullUrl);
        }
    }
}