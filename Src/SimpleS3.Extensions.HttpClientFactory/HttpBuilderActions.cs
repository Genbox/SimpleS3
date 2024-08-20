namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

public class HttpBuilderActions
{
    public IList<Action<IServiceProvider, HttpClient>> HttpClientActions { get; } = new List<Action<IServiceProvider, HttpClient>>();
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; } = new List<Action<IServiceProvider, HttpClientHandler>>();
}