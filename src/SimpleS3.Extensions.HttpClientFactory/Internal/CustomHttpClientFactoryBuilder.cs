using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;

internal class CustomHttpClientFactoryBuilder : ServiceBuilderBase, IHttpClientBuilder
{
    public CustomHttpClientFactoryBuilder(IServiceCollection services, string? name = null) : base(services, name) { }
}