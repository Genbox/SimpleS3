using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient;

/// <summary>Provides the <c>IHttpClientBuilder</c> API.</summary>
public interface IHttpClientBuilder
{
    /// <summary>Provides the <c>Services</c> API.</summary>
    IServiceCollection Services { get; }

    /// <summary>Provides the <c>Name</c> API.</summary>
    string Name { get; }
}