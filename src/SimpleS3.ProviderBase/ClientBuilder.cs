using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3
{
    public class ClientBuilder : ServiceBuilderBase, IClientBuilder
    {
        public ClientBuilder(IServiceCollection services, IHttpClientBuilder httpBuilder, ICoreBuilder coreBuilder) : base(services)
        {
            HttpBuilder = httpBuilder;
            CoreBuilder = coreBuilder;
        }

        public IHttpClientBuilder HttpBuilder { get; }
        public ICoreBuilder CoreBuilder { get; }
    }
}