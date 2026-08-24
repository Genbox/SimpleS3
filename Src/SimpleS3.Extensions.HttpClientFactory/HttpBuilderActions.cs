namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

/// <summary>Provides the <c>HttpBuilderActions</c> API.</summary>
public class HttpBuilderActions
{
    /// <summary>Provides the <c>HttpClientActions</c> API.</summary>
    public IList<Action<IServiceProvider, HttpClient>> HttpClientActions { get; } = new List<Action<IServiceProvider, HttpClient>>();
    /// <summary>Provides the <c>HttpHandlerActions</c> API.</summary>
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; } = new List<Action<IServiceProvider, HttpClientHandler>>();
}