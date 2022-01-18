using System.Net;
using Microsoft.Extensions.Options;
using HttpVersion = Genbox.SimpleS3.Core.Common.HttpVersion;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

public class HttpClientFactoryConfig
{
    /// <summary> Set to 'true' if you want the proxy that is defined to be used. Otherwise set it to 'false'. Defaults to 'false'. </summary>
    public bool UseProxy { get; set; }

    /// <summary>Use this to set a proxy to which all requests should be sent through.</summary>
    public IWebProxy? Proxy { get; set; }

    /// <summary>Controls how long a handler should stay active before a new one is created.</summary>
    public TimeSpan HandlerLifetime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// This can be used to set the name of the HttpClient that should be created through HttpClientFactory. You only
    /// need to set this in case you are using HttpClientFactory to create multiple HttpClient instances with different
    /// configuration. This value defaults to string.Empty.
    /// </summary>
    public string HttpClientName { get; set; } = Options.DefaultName;

    /// <summary>Set the HTTP version that the driver should use.</summary>
    public HttpVersion HttpVersion { get; set; }
}