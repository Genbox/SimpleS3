namespace Genbox.SimpleS3.Extensions.HttpClient;

public class HttpBuilderActions
{
    public IList<Action<IServiceProvider, System.Net.Http.HttpClient>> HttpClientActions { get; } = new List<Action<IServiceProvider, System.Net.Http.HttpClient>>();
    public IList<Action<IServiceProvider, HttpClientHandler>> HttpHandlerActions { get; } = new List<Action<IServiceProvider, HttpClientHandler>>();
}