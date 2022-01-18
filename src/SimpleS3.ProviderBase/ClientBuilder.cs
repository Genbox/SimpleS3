using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.ProviderBase;

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