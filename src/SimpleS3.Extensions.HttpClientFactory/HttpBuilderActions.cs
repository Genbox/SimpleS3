namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

public class HttpBuilderActions
{
    public HttpBuilderActions()
    {
        HttpClientActions = new List<Action<IServiceProvider, HttpClient>>();
        HttpHandlerActions = new List<Action<IServiceProvider, HttpClientHandler>>();
    }

    public IList<Action<IServiceProvider, HttpClient>> HttpClientActions { get; }
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; }
}