using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Internal;

internal class CustomHttpClientBuilder : ServiceBuilderBase, IHttpClientBuilder
{
    public CustomHttpClientBuilder(IServiceCollection services, string? name = null) : base(services, name) { }
}