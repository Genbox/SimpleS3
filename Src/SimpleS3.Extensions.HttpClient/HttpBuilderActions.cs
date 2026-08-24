namespace Genbox.SimpleS3.Extensions.HttpClient;

/// <summary>Provides the <c>HttpBuilderActions</c> API.</summary>
public class HttpBuilderActions
{
    /// <summary>Provides the <c>HttpClientActions</c> API.</summary>
    public IList<Action<IServiceProvider, System.Net.Http.HttpClient>> HttpClientActions { get; } = new List<Action<IServiceProvider, System.Net.Http.HttpClient>>();
    /// <summary>Provides the <c>HttpHandlerActions</c> API.</summary>
    public IList<Action<IServiceProvider, HttpMessageHandler>> HttpHandlerActions { get; } = new List<Action<IServiceProvider, HttpMessageHandler>>();
}