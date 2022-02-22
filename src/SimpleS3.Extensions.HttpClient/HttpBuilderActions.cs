namespace Genbox.SimpleS3.Extensions.HttpClient;

public class HttpBuilderActions
{
    public HttpBuilderActions()
    {
        HttpClientActions = new List<Action<IServiceProvider, System.Net.Http.HttpClient>>();
        HttpHandlerActions = new List<Action<IServiceProvider, HttpClientHandler>>();
    }

    public IList<Action<IServiceProvider, System.Net.Http.HttpClient>> HttpClientActions { get; }
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; }
}