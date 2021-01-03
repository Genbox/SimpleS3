using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2
{
    //https://www.backblaze.com/b2/docs/s3_compatible_api.html

    public class B2UrlBuilder : UrlBuilderBase
    {
        public B2UrlBuilder(IOptions<Config> options) : base(options) { }

        protected override void AppendVirtualHost(StringBuilder sb, Config config, string? bucketName)
        {
            //https://bucketname.s3.us-west-001.backblazeb2.com

            if (bucketName == null)
                sb.Append("s3.").Append(config.RegionCode).Append(".backblazeb2.com");
            else
                sb.Append(bucketName).Append(".s3.").Append(config.RegionCode).Append(".backblazeb2.com");
        }

        protected override void AppendPathStyle(StringBuilder sb, Config config, string? bucketName)
        {
            //https://s3.us-west-001.backblazeb2.com/bucketname
            sb.Append("s3.").Append(config.RegionCode).Append(".backblazeb2.com");
        }
    }
}