using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Abstracts
{
    internal class S3ClientBuilder : ServiceBuilderBase, IS3ClientBuilder
    {
        public S3ClientBuilder(IServiceCollection services, IHttpClientBuilder httpBuilder, ICoreBuilder coreBuilder) : base(services)
        {
            HttpBuilder = httpBuilder;
            CoreBuilder = coreBuilder;
        }

        public IHttpClientBuilder HttpBuilder { get; }
        public ICoreBuilder CoreBuilder { get; }
    }
}