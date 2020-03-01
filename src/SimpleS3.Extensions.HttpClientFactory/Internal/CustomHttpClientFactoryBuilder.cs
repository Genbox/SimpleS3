using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Internal
{
    internal class CustomHttpClientFactoryBuilder : IHttpClientBuilder
    {
        public CustomHttpClientFactoryBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public string Name { get; }
        public IServiceCollection Services { get; }
    }
}