using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IS3ClientBuilder
    {
        IServiceCollection Services { get; }

        IHttpClientBuilder HttpBuilder { get; }

        ICoreBuilder CoreBuilder { get; }
    }
}