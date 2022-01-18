using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.ProviderBase.Abstracts;

public interface IClientBuilder
{
    IServiceCollection Services { get; }

    IHttpClientBuilder HttpBuilder { get; }

    ICoreBuilder CoreBuilder { get; }
}