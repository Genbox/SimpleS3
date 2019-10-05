using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IS3ClientBuilder
    {
        IServiceCollection Services { get; }
    }
}