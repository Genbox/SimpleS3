namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

public class HttpBuilderActions
{
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; }
    public IList<Action<IServiceProvider, HttpClient>> HttpClientActions { get; }

    public HttpBuilderActions()
    {
        HttpHandlerActions = new List<Action<IServiceProvider, HttpClientHandler>>();
        HttpClientActions = new List<Action<IServiceProvider, HttpClient>>();
    }
}