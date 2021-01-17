using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.AwsS3
{
    public class AwsUrlBuilder : UrlBuilderBase
    {
        public AwsUrlBuilder(IOptions<Config> options) : base(options) { }

        protected override void AppendVirtualHost(StringBuilder sb, Config config, string? bucketName)
        {
            if (bucketName == null)
                sb.Append("s3.").Append(config.RegionCode).Append(".amazonaws.com");
            else
                sb.Append(bucketName).Append(".s3.").Append(config.RegionCode).Append(".amazonaws.com");
        }

        protected override void AppendPathStyle(StringBuilder sb, Config config, string? bucketName)
        {
            sb.Append("s3.").Append(config.RegionCode).Append(".amazonaws.com");
        }
    }
}