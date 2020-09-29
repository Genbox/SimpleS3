using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Abstracts
{
    internal class S3ClientBuilder : ServiceBuilderBase, IS3ClientBuilder
    {
        public S3ClientBuilder(IServiceCollection services) : base(services) { }

        public IHttpClientBuilder? HttpBuilder { get; set; }
        public ICoreBuilder? CoreBuilder { get; set; }
    }
}