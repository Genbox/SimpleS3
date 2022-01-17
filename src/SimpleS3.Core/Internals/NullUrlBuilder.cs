using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals
{
    public class NullUrlBuilder : UrlBuilderBase
    {
        public NullUrlBuilder(IOptions<Config> options) : base(options) { }

        protected override void AppendVirtualHost(StringBuilder sb, Config config, string? bucketName)
        {
        }

        protected override void AppendPathStyle(StringBuilder sb, Config config, string? bucketName)
        {
        }
    }
}