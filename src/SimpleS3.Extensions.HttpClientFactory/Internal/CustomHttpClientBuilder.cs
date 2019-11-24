using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Internal
{
    internal class CustomHttpClientBuilder : IHttpClientBuilder
    {
        public CustomHttpClientBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public string Name { get; }
        public IServiceCollection Services { get; }
    }
}