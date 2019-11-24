using System;
using System.Net;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory
{
    public class HttpClientFactoryConfig
    {
        public bool UseProxy { get; set; }
        public IWebProxy Proxy { get; set; }
        public TimeSpan HandlerLifetime { get; set; } = TimeSpan.FromMinutes(5);
    }
}