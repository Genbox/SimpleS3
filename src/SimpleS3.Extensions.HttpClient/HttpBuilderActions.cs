namespace Genbox.SimpleS3.Extensions.HttpClient;

public class HttpBuilderActions
{
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; }
    public IList<Action<IServiceProvider, System.Net.Http.HttpClient>> HttpClientActions { get; }

    public HttpBuilderActions()
    {
        HttpHandlerActions = new List<Action<IServiceProvider, HttpClientHandler>>();
        HttpClientActions = new List<Action<IServiceProvider, System.Net.Http.HttpClient>>();
    }
}