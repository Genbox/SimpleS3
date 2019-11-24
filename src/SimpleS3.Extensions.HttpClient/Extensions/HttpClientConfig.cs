using System.Net;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public class HttpClientConfig
    {
        public bool UseProxy { get; set; }
        public IWebProxy Proxy { get; set; }
    }
}