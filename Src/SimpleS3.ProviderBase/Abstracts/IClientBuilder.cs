using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.ProviderBase.Abstracts;

/// <summary>Provides the <c>IClientBuilder</c> API.</summary>
public interface IClientBuilder
{
    /// <summary>Provides the <c>Services</c> API.</summary>
    IServiceCollection Services { get; }

    /// <summary>Provides the <c>HttpBuilder</c> API.</summary>
    IHttpClientBuilder HttpBuilder { get; }

    /// <summary>Provides the <c>CoreBuilder</c> API.</summary>
    ICoreBuilder CoreBuilder { get; }
}