using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.ProviderBase;

/// <summary>Provides the <c>ClientBuilder</c> API.</summary>
public class ClientBuilder(IServiceCollection services, IHttpClientBuilder httpBuilder, ICoreBuilder coreBuilder, string? name = null) : ServiceBuilderBase(services, name), IClientBuilder
{
    /// <summary>Provides the <c>HttpBuilder</c> API.</summary>
    public IHttpClientBuilder HttpBuilder { get; } = httpBuilder;
    /// <summary>Provides the <c>CoreBuilder</c> API.</summary>
    public ICoreBuilder CoreBuilder { get; } = coreBuilder;
}