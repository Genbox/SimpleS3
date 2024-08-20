using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.ProviderBase;

public class ClientBuilder(IServiceCollection services, IHttpClientBuilder httpBuilder, ICoreBuilder coreBuilder, string? name = null) : ServiceBuilderBase(services, name), IClientBuilder
{
    public IHttpClientBuilder HttpBuilder { get; } = httpBuilder;
    public ICoreBuilder CoreBuilder { get; } = coreBuilder;
}