using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory
{
    public class HttpClientFactoryNetworkDriverConfig
    {
        /// <summary>This can be used to set the name of the HttpClient that should be created through HttpClientFactory. You only need to set this in case you
        /// are using HttpClientFactory to create multiple HttpClient instances with different configuration. This value defaults to string.Empty.</summary>
        public string HttpClientName { get; set; } = Options.DefaultName;
    }
}