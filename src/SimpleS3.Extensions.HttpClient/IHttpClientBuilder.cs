using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient;

public interface IHttpClientBuilder
{
    IServiceCollection Services { get; }
}