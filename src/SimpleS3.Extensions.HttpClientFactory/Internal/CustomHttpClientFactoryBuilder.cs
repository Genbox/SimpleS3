using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;

internal class CustomHttpClientFactoryBuilder : IHttpClientBuilder
{
    public CustomHttpClientFactoryBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public string Name { get; } = Options.DefaultName;
    public IServiceCollection Services { get; }
}