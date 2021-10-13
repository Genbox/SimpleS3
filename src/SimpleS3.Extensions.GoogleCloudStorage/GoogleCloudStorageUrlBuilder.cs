using System;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage
{
    // https://cloud.google.com/storage/docs/request-endpoints#xml-api

    public class GoogleCloudStorageUrlBuilder : UrlBuilderBase
    {
        public GoogleCloudStorageUrlBuilder(IOptions<Config> options) : base(options) { }

        protected override void AppendVirtualHost(StringBuilder sb, Config config, string? bucketName)
        {
            //https://BUCKET_NAME.storage.googleapis.com

            if (bucketName == null)
                throw new InvalidOperationException("Not supported yet");

            sb.Append(bucketName).Append(".storage.googleapis.com");
        }

        protected override void AppendPathStyle(StringBuilder sb, Config config, string? bucketName)
        {
            //https://storage.googleapis.com
            sb.Append("storage.googleapis.com");
        }
    }
}